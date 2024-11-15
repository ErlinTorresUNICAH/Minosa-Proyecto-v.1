using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Minosa_Proyecto_v._1.Models;
using System.Data;

/// <summary>
/// Clase heredada de BackgroundService para tener un servicio en segundo plano.
/// </summary>
public class ActividadBackgroundService : BackgroundService
{
    private readonly string _connectionString;
    private readonly ILogger<ActividadBackgroundService> _logger;

    public ActividadBackgroundService(IConfiguration configuration, ILogger<ActividadBackgroundService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Starting activity device scan at: {time}", DateTimeOffset.Now);

            var dispositivos = await ObtenerActividadDispositivosAsync();
           /* var dispositivosHistory = await ObtenerHistorialDispositivosAsync();*/
            await EscanearDispositivosConNmapAsync(dispositivos);
            await ActualizarEstadoPingAsync(dispositivos);




            _logger.LogInformation("Iniciando inserción de historial de ping...");
            await InsertarHistorialPingAsync(dispositivos);
            _logger.LogInformation("Finalizada inserción de historial de ping.");



            _logger.LogInformation("Completed activity device scan at: {time}", DateTimeOffset.Now);

            await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);

        }

    }

    /// Obtiene una lista de dispositivos de actividad desde la base de datos.
    private async Task<List<Actividad>> ObtenerActividadDispositivosAsync()
    {
        List<Actividad> dispositivos = new List<Actividad>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerActividadConTiempo]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dispositivos.Add(new Actividad
                        {
                            ID_Equipo = reader["ID_equipo"] != DBNull.Value ? Convert.ToInt32(reader["ID_equipo"]) : 0,
                            DireccionIP = reader["DireccionIP"] != DBNull.Value ? reader["DireccionIP"].ToString() : string.Empty,
                            Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : string.Empty,
                            DescripcionEquipo = reader["DescripcionEquipo"] != DBNull.Value ? reader["DescripcionEquipo"].ToString() : string.Empty,
                            TipoEquipo = reader["TipoEquipo"] != DBNull.Value ? reader["TipoEquipo"].ToString() : string.Empty,
                            Ping = reader["Ping"] != DBNull.Value && Convert.ToBoolean(reader["Ping"]),
                            UltimaHoraPing = reader["UltimaHoraPing"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UltimaHoraPing"]) : null
                        });
                    }
                }
            }
        }

        return dispositivos;
    }

    // scanner de las Ip
    private async Task EscanearDispositivosConNmapAsync(List<Actividad> dispositivos)
    {
        foreach (var dispositivo in dispositivos)
        {
            _logger.LogInformation("Pinging {DireccionIP} at {Time}", dispositivo.DireccionIP, DateTime.Now);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                dispositivo.Ping = await EjecutarNmapPingAsync(dispositivo.DireccionIP) == "Activo";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pinging IP: {DireccionIP}", dispositivo.DireccionIP);
                dispositivo.Ping = false;
            }

            stopwatch.Stop();
            _logger.LogInformation("Ping to {DireccionIP} {Result} in {Seconds} seconds",
                dispositivo.DireccionIP,
                dispositivo.Ping ? "succeeded" : "failed",
                stopwatch.Elapsed.TotalSeconds);

            dispositivo.UltimaHoraPing = DateTime.Now;
            _logger.LogInformation("UltimaHoraPing asignada a {Time} para {DireccionIP}", dispositivo.UltimaHoraPing, dispositivo.DireccionIP);
        }

        
    }

    // Ejecutor del programa nmap
    private async Task<string> EjecutarNmapPingAsync(string direccionIP)
    {
        return await Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files (x86)\Nmap\nmap.exe";
            process.StartInfo.Arguments = $"-sn {direccionIP}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains("Host is up") ? "Activo" : "Inactivo";
        });
    }

    /*private async Task ActualizarEstadoPingAsync(List<Actividad> dispositivos)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            foreach (var dispositivo in dispositivos)
            {
                string query = "UPDATE DireccionesIp SET ping = @Ping, UltimaHoraPing = @UltimaHoraPing WHERE IPV4 = @DireccionIP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Ping", dispositivo.Ping ? 1 : 0);
                    cmd.Parameters.AddWithValue("@UltimaHoraPing", dispositivo.UltimaHoraPing ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DireccionIP", dispositivo.DireccionIP);

                    try
                    {
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            _logger.LogWarning("No rows were updated for IP: {DireccionIP}", dispositivo.DireccionIP);
                        }
                        else
                        {
                            _logger.LogInformation("Updated {rowsAffected} rows for IP: {DireccionIP}", rowsAffected, dispositivo.DireccionIP);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating database for IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                }
            }
        }
        //await CuentaRegresivaCincoMinutosAsync();
    }
*/

    //actualiza la actividad del ping
    private async Task ActualizarEstadoPingAsync(List<Actividad> dispositivos)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            foreach (var dispositivo in dispositivos)
            {
                // Actualizar el estado del ping en la tabla principal
                string updateQuery = "UPDATE DireccionesIp SET ping = @Ping, UltimaHoraPing = @UltimaHoraPing WHERE IPV4 = @DireccionIP";
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@Ping", dispositivo.Ping ? 1 : 0);
                    updateCmd.Parameters.AddWithValue("@UltimaHoraPing", dispositivo.UltimaHoraPing ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@DireccionIP", dispositivo.DireccionIP);

                    try
                    {
                        int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            _logger.LogInformation("Updated {rowsAffected} rows for IP: {DireccionIP}", rowsAffected, dispositivo.DireccionIP);
                        }
                        else
                        {
                            _logger.LogWarning("No rows were updated for IP: {DireccionIP}", dispositivo.DireccionIP);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating database for IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                }
            }
        }
    }


    private async Task InsertarHistorialPingAsync(List<Actividad> dispositivos)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            foreach (var dispositivo in dispositivos)
            {
                // Insertar el historial de ping en la tabla de historial
                using (SqlCommand insertCmd = new SqlCommand("[dbo].[P_InsertarHistorialPing]", conn))
                {
                    insertCmd.CommandType = CommandType.StoredProcedure;
                    insertCmd.Parameters.AddWithValue("@ip", dispositivo.DireccionIP); // Asegúrate de que esto corresponda con el ID correcto de la dirección IP
                    insertCmd.Parameters.AddWithValue("@HoraPing", dispositivo.UltimaHoraPing);
                    insertCmd.Parameters.AddWithValue("@ResultadoPing", dispositivo.Ping ? 1 : 0);
                    

                    try
                    {
                        await insertCmd.ExecuteNonQueryAsync();
                        _logger.LogInformation("Historial de ping insertado para IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inserting ping history for IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                }
            }
        }
    }
    private async Task<List<Actividad>> ObtenerHistorialDispositivosAsync()
    {
        List<Actividad> dispositivosHistory = new List<Actividad>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerHistorialPings]", conn)) // Ajusta el nombre del procedimiento almacenado si es necesario
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dispositivosHistory.Add(new Actividad
                        {
                            ID_HistorialPing = reader["ID_HistorialPing"] != DBNull.Value ? Convert.ToInt32(reader["ID_HistorialPing"]) : 0,
                            DireccionIP = reader["ip"] != DBNull.Value ? reader["ip"].ToString() : string.Empty,
                            UltimaHoraPing = reader["HoraPing"] != DBNull.Value ? Convert.ToDateTime(reader["HoraPing"]) : DateTime.MinValue,
                            Ping = reader["ResultadoPing"] != DBNull.Value && Convert.ToBoolean(reader["ResultadoPing"])
                        });
                    }
                }
            }
        }

        return dispositivosHistory;
    }


    //cuenta regresiva para control en el cmd
    private async Task CuentaRegresivaCincoMinutosAsync()
    {
        int totalSegundos = 2 * 60; // 5 minutes in seconds

        while (totalSegundos > 0)
        {
            TimeSpan tiempoRestante = TimeSpan.FromSeconds(totalSegundos);
            _logger.LogInformation("Tiempo restante: {Minutes} minutos {Seconds} segundos",
                tiempoRestante.Minutes, tiempoRestante.Seconds);

            await Task.Delay(1000); // Wait for 1 second
            totalSegundos--;
        }

        _logger.LogInformation("Han pasado los 5 minutos.");
    }
}
