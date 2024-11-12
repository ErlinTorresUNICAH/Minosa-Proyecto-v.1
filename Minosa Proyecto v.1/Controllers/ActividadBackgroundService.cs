using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Minosa_Proyecto_v._1.Models;
using System.Data;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class ActividadBackgroundService : BackgroundService
{
    private readonly string _connectionString;

    public ActividadBackgroundService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var dispositivos = ObtenerActividadDispositivos();
            await EscanearDispositivosConNmapAsync(dispositivos);
            ActualizarEstadoPing(dispositivos);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private List<Actividad> ObtenerActividadDispositivos()
    {
        List<Actividad> dispositivos = new List<Actividad>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerActividad]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dispositivos.Add(new Actividad
                        {
                            ID_Equipo = Convert.ToInt32(reader["ID_equipo"]),
                            DireccionIP = reader["DireccionIP"].ToString(),
                            Area = reader["Area"].ToString(),
                            DescripcionEquipo = reader["DescripcionEquipo"].ToString(),
                            TipoEquipo = reader["TipoEquipo"].ToString(),
                            Ping = Convert.ToBoolean(reader["Ping"])
                        });
                    }
                }
            }
        }

        return dispositivos;
    }

    private async Task EscanearDispositivosConNmapAsync(List<Actividad> dispositivos)
    {
        foreach (var dispositivo in dispositivos)
        {
            Console.WriteLine($"Pinging {dispositivo.DireccionIP} at {DateTime.Now}...");
            var stopwatch = Stopwatch.StartNew();
            dispositivo.Ping = await EjecutarNmapPingAsync(dispositivo.DireccionIP) == "Activo";
            stopwatch.Stop();
            Console.WriteLine($"Ping to {dispositivo.DireccionIP} {(dispositivo.Ping ? "succeeded" : "failed")} in {stopwatch.Elapsed.TotalSeconds} seconds");

            // Guardar la hora del último ping
            dispositivo.UltimaHoraPing = DateTime.Now;
            
        }
        CuentaRegresivaCincoMinutosAsync();
    }

    // Contador para verificar si han pasado 5 minutos desde el último ping
    private async Task CuentaRegresivaCincoMinutosAsync()
    {
        int totalSegundos = 5 * 60; // 5 minutos en segundos

        while (totalSegundos > 0)
        {
            /*Console.Clear();*/ // Limpiar la consola para que se vea la cuenta regresiva "en tiempo real"
            TimeSpan tiempoRestante = TimeSpan.FromSeconds(totalSegundos);
            Console.WriteLine($"Tiempo restante: {tiempoRestante.Minutes} minutos {tiempoRestante.Seconds} segundos");

            await Task.Delay(1000); // Esperar 1 segundo
            totalSegundos--;
        }

        Console.WriteLine("Han pasado los 5 minutos.");
    }



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

    private void ActualizarEstadoPing(List<Actividad> dispositivos)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            foreach (var dispositivo in dispositivos)
            {
                string query = "UPDATE DireccionesIp SET ping = @Ping WHERE IPV4 = @DireccionIP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Ping", dispositivo.Ping ? 1 : 0);
                    cmd.Parameters.AddWithValue("@DireccionIP", dispositivo.DireccionIP);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
