/*using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using System.Data;
using System.Diagnostics;

public class ActividadController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public ActividadController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<ActionResult> Index()
    {
        // Retrieve devices from the database
        var dispositivos = ObtenerActividadDispositivos();

        // Perform asynchronous scanning with Nmap
        await EscanearDispositivosConNmapAsync(dispositivos);

        // Option to update the ping status in the database
        ActualizarEstadoPing(dispositivos);

        return View(dispositivos); // Returns the updated list to the Index view
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
            Console.WriteLine($"Pinging {dispositivo.DireccionIP}...");
            dispositivo.Ping = await EjecutarNmapPingAsync(dispositivo.DireccionIP) == "Activo";
            Console.WriteLine($"Ping to {dispositivo.DireccionIP} {(dispositivo.Ping ? "succeeded" : "failed")}");
        }
    }

    private async Task<string> EjecutarNmapPingAsync(string direccionIP)
    {
        return await Task.Run(() =>
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files (x86)\Nmap\nmap.exe"; // Path to Nmap
            process.StartInfo.Arguments = $"-sn {direccionIP}"; // Ping scan
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Check if ping was successful by analyzing the Nmap output
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
}*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using System.Data;

public class ActividadController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public ActividadController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<ActionResult> Index()
    {
        var dispositivos = ObtenerActividadDispositivos();
        return View(dispositivos);
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
}

