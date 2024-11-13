

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
                            ID_Equipo = reader["ID_equipo"] != DBNull.Value ? Convert.ToInt32(reader["ID_equipo"]) : 0, // Default to 0 if null
                            DireccionIP = reader["DireccionIP"] != DBNull.Value ? reader["DireccionIP"].ToString() : string.Empty, // Default to empty string if null
                            Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : string.Empty,
                            DescripcionEquipo = reader["DescripcionEquipo"] != DBNull.Value ? reader["DescripcionEquipo"].ToString() : string.Empty,
                            TipoEquipo = reader["TipoEquipo"] != DBNull.Value ? reader["TipoEquipo"].ToString() : string.Empty,
                            Ping = reader["Ping"] != DBNull.Value && Convert.ToBoolean(reader["Ping"]) // Default to false if null
                        });
                    }
                }
            }
        }

        return dispositivos;
    }

}

/*
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
            using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerActividadTotal]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dispositivos.Add(new Actividad
                        {
                            ID_Equipo = reader["ID_equipo"] != DBNull.Value ? Convert.ToInt32(reader["ID_equipo"]) : 0,
                            DireccionIP = reader["DireccionIP"] != DBNull.Value ? reader["DireccionIP"].ToString() : string.Empty,
                            Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : string.Empty,
                            DescripcionEquipo = reader["DescripcionEquipo"] != DBNull.Value ? reader["DescripcionEquipo"].ToString() : string.Empty,
                            TipoEquipo = reader["TipoEquipo"] != DBNull.Value ? reader["TipoEquipo"].ToString() : string.Empty,
                            Ping = reader["Ping"] != DBNull.Value && Convert.ToBoolean(reader["Ping"]),
                            UltimaHoraPing = (DateTime)(reader["UltimaHoraPing"] != DBNull.Value ? Convert.ToDateTime(reader["UltimaHoraPing"]) : (DateTime?)null)
                        });
                    }
                }
            }
        }

        return dispositivos;
    }
}
*/