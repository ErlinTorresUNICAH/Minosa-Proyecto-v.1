/*using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;

namespace Minosa_Proyecto_v._1.Controllers
{
    public class ActividadController : Controller
    {
        private readonly string _connectionString;

        public ActividadController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Acción para mostrar la vista inicial
        public IActionResult Index()
        {
            var dispositivos = ObtenerActividadDispositivos(); // Obtener datos iniciales

          *//*  var historialPings = ObtenerHistorialPings(); // Obtener historial de pings
            ViewBag.HistorialPings = historialPings; // Pasar historial a la vista*//*
            return View(dispositivos); // Pasar datos a la vista
        }

        // Acción para obtener datos de ping de forma dinámica mediante AJAX
        [HttpGet]
        public JsonResult ObtenerDatosPing()
        {
            var dispositivos = ObtenerActividadDispositivos();
            return Json(dispositivos);
        }

        private List<Actividad> ObtenerActividadDispositivos()
        {
            List<Actividad> dispositivos = new List<Actividad>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerActividadConTiempo]", conn))
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
                                UltimaHoraPing = reader["UltimaHoraPing"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UltimaHoraPing"]) : null
                            });
                        }
                    }
                }
            }

            return dispositivos;
        }


    }
}
*/

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Minosa_Proyecto_v._1.Controllers
{
    public class ActividadController : Controller
    {
        private readonly string _connectionString;

        public ActividadController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Acción para mostrar la vista inicial
        public IActionResult Index()
        {
            var dispositivos = ObtenerActividadDispositivos(); // Obtener datos iniciales
            var historialPings = ObtenerHistorialPings(); // Obtener historial de pings
            ViewBag.HistorialPings = historialPings; // Pasar historial a la vista
            return View(dispositivos); // Pasar datos a la vista
        }

        public IActionResult IndexGraficos()
        {
            var dispositivos = ObtenerActividadDispositivos(); // Obtener datos iniciales
            var historialPings = ObtenerHistorialPings(); // Obtener historial de pings
            ViewBag.HistorialPings = historialPings; // Pasar historial a la vista
            return View("IndexGraficos", dispositivos); // Pasar datos a la vista
        }

        public JsonResult ObtenerHistorialPingsAJAX()
        {
            var historialPings = ObtenerHistorialPings(); // Obtener el historial actualizado de pings
            return Json(historialPings);
        }

        private List<Actividad> ObtenerActividadDispositivos()
        {
            
            List<Actividad> dispositivos = new List<Actividad>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerActividadConTiempo]", conn))
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
                                    UltimaHoraPing = reader["UltimaHoraPing"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UltimaHoraPing"]) : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dispositivos;
        }

        private List<Actividad> ObtenerHistorialPings(int? idIp = null)
        {
            List<Actividad> historialPings = new List<Actividad>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerHistorialPings]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ip", idIp.HasValue ? (object)idIp.Value : DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historialPings.Add(new Actividad
                            {
                                ID_HistorialPing = Convert.ToInt32(reader["ID_HistorialPing"]),
                                DireccionIP = (string)reader["ip"],
                                UltimaHoraPing = Convert.ToDateTime(reader["HoraPing"]),
                                Ping = Convert.ToBoolean(reader["ResultadoPing"])
                            });
                        }
                    }
                }
            }

            return historialPings;
        }
    }
}
