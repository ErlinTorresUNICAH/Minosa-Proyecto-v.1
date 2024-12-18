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
using System.Net.Mail;
using System.Net;
using MimeKit;

/// <summary>
/// Clase heredada de BackgroundService para tener un servicio en segundo plano.
/// </summary>
public class ActividadBackgroundService(IConfiguration configuration, ILogger<ActividadBackgroundService> logger) : BackgroundService
{
    private readonly IConfiguration _configuration;

    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");
    
    private readonly ILogger<ActividadBackgroundService> _logger = logger;

    //Llama a todas las funcion en orden para su control
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        while (!stoppingToken.IsCancellationRequested)
        {
           
            var dispositivos = await ObtenerActividadDispositivosAsync();
            await EscanearDispositivosConNmapAsync(dispositivos);
            await ActualizarEstadoPingAsync(dispositivos);
            await InsertarHistorialPingAsync(dispositivos);
            await EnviarCorreoDispositivosDesconectadosAsync(dispositivos);


            //cambiar este parametro en base del tiempo que se quiere que se ejecute los pings en minutos
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

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
            
            var stopwatch = Stopwatch.StartNew();

            try
            {
                dispositivo.Ping = (await EjecutarNmapPingAsync(dispositivo.DireccionIP)) == "Activo";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pinging IP: {DireccionIP}", dispositivo.DireccionIP);
                dispositivo.Ping = false;
            }

            stopwatch.Stop();
            

            dispositivo.UltimaHoraPing = DateTime.Now;
            
        }
    }

    // Ejecutor del programa nmap
    private async Task<string> EjecutarNmapPingAsync(string direccionIP)
    {
        return await Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c ping -n 4 {direccionIP}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Verificar si la salida contiene "Reply from" al menos una vez
            bool activo = output.Contains("Reply from");
            return activo ? "Activo" : "Inactivo";
        });
    }

    //actualiza la actividad del ping
    private async Task ActualizarEstadoPingAsync(List<Actividad> dispositivos)
    {

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            foreach (var dispositivo in dispositivos)
            {

                


                // Actualizar el estado del ping en la tabla principal
                string updateQuery = "P_ActualizarPingDireccionIp";
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.CommandType = CommandType.StoredProcedure;
                    updateCmd.Parameters.AddWithValue("@Ping", dispositivo.Ping ? 1 : 0);
                    
                    updateCmd.Parameters.AddWithValue("@UltimaHoraPing", dispositivo.UltimaHoraPing);
                    updateCmd.Parameters.AddWithValue("@DireccionIP", dispositivo.DireccionIP);
                    
                    try
                    {
                        int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating database for IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                }
            }
        }
    }

    //actualiza la historial del ping esto para la vista se actualice automaticamente
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
                    insertCmd.Parameters.AddWithValue("@ip", dispositivo.DireccionIP);
                    insertCmd.Parameters.AddWithValue("@HoraPing", dispositivo.UltimaHoraPing);
                    insertCmd.Parameters.AddWithValue("@ResultadoPing", dispositivo.Ping ? 1 : 0);

                    
                    try
                    {
                        await insertCmd.ExecuteNonQueryAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inserting ping history for IP: {DireccionIP}", dispositivo.DireccionIP);
                    }
                }
            }
        }
    }

    // Método para enviar correos electrónicos con dispositivos desconectados
    private async Task EnviarCorreoDispositivosDesconectadosAsync(List<Actividad> dispositivos)
    {
        var correoSettings = ObtenerCorreoSettings();
        if (correoSettings == null)
        {
            _logger.LogError("No se pudo obtener la configuración de correo desde la base de datos.");
            return;
        }

        var smtpServer = correoSettings.SmtpServer;
        var smtpPort = correoSettings.SmtpPort;
        var emailFrom = correoSettings.EmailFrom;
        var emailPassword = correoSettings.EmailPassword;

        var dispositivosDesconectados = dispositivos.Where(d => !d.Ping).ToList();
        if (!dispositivosDesconectados.Any())
        {
            _logger.LogInformation("No hay dispositivos desconectados para enviar en el correo.");
            return;
        }

        var correosDestinatarios = await ObtenerCorreosDestinatariosAsync();
        if (!dispositivosDesconectados.Any())
        {
            _logger.LogInformation("No hay dispositivos desconectados para enviar en el correo.");
            return;
        }

        var message = new MimeMessage();

        // Configurar remitente 
        message.From.Add(new MailboxAddress("NetGuardião", emailFrom));
        
        for (int i = 0; i < correosDestinatarios.Count; i++)
        {
            var nombre = correosDestinatarios[i].Nombre_Destinatario;
            var correo = correosDestinatarios[i].Correo_Destinatario;
            message.Bcc.Add(new MailboxAddress(nombre, correo));
            if (!string.IsNullOrEmpty(correo))
            {
                message.Bcc.Add(new MailboxAddress(nombre ?? "Destinatario", correo));
            }
        }
        
        // Asunto del correo
        message.Subject = "Alerta: Dispositivos desconectados";

        // Cuerpo del mensaje
        message.Body = new TextPart("html")
        {
            Text = GenerarCuerpoCorreoHTML(dispositivosDesconectados)
        };

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            try
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Conectar al servidor SMTP
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                // Autenticación
                await client.AuthenticateAsync(emailFrom, emailPassword);

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico.");
            }
        }
    }
    private CorreoSettings ObtenerCorreoSettings()
    {

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("P_ObtenerCorreoSettings", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CorreoSettings
                            {
                                SmtpServer = reader["SmtpServer"].ToString(),
                                SmtpPort = Convert.ToInt32(reader["SmtpPort"]),
                                EmailFrom = reader["EmailFrom"].ToString(),
                                EmailPassword = reader["EmailPassword"].ToString()
                            };
                        }
                    }
                }
            }
        }
        catch
        {
            

        }
        return null; 
    }

    // Cuerpo del correo en formato HTML
    private string GenerarCuerpoCorreoHTML(List<Actividad> dispositivosDesconectados)
    {
        var cuerpo = @"
    <html>
    <head>
        <style>
            table {
                width: 100%;
                border-collapse: collapse;
                font-family: Arial, sans-serif;
                font-size: 14px;
            }
            th, td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }
            th {
                background-color: #f2f2f2;
                color: #333;
            }
            tr:nth-child(even) {
                background-color: #f9f9f9;
            }
            tr:hover {
                background-color: #f1f1f1;
            }
            h1 {
                font-family: Arial, sans-serif;
                color: #333;
            }
        </style>
    </head>
    <body>
        <h1>Dispositivos Desconectados</h1>
        <table>
            <thead>
                <tr>
                    <th>IP</th>
                    <th>Descripción</th>
                    <th>Área</th>
                    <th>Última Hora Ping</th>
                </tr>
            </thead>
            <tbody>";

        foreach (var dispositivo in dispositivosDesconectados)
        {
            cuerpo += $@"
                <tr>
                    <td>{dispositivo.DireccionIP}</td>
                    <td>{dispositivo.DescripcionEquipo}</td>
                    <td>{dispositivo.Area}</td>
                    <td>{(dispositivo.UltimaHoraPing.HasValue ? dispositivo.UltimaHoraPing.Value.ToString("g") : "N/A")}</td>
                </tr>";
        }

        cuerpo += @"
            </tbody>
        </table>
    </body>
    </html>";

        return cuerpo;
    }

    // Funcion para optener los correos
    private async Task<List<Actividad>> ObtenerCorreosDestinatariosAsync()
    {
        var correos = new List<Actividad>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("P_ListaDeCorreosAlertas", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        correos.Add(new Actividad
                        {
                            Nombre_Destinatario = reader["Nombre_Destinatario"]?.ToString() ?? string.Empty,
                            Correo_Destinatario = reader["Correo_Destinatario"]?.ToString() ?? string.Empty


                        });
                    }
                }
            }
        }

        return correos;
    }
    // 
    private async Task<List<Actividad>> ObtenerHistorialDispositivosAsync()
    {
        List<Actividad> dispositivosHistory = new List<Actividad>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerHistorialPings]", conn)) 
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

            await Task.Delay(1000); 
            totalSegundos--;
        }

        _logger.LogInformation("Han pasado los 5 minutos.");
    }
}