using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using iTextSharp.text;

namespace Minosa_Proyecto_v._1.Controllers
{
    // Clase necesaria para poder ver tanto el historial como para ver la actividad, complemento de ActividadBackgroundService
    public class ActividadController : Controller
    {
        private readonly string _connectionString;

        public ActividadController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        

        // Funcion para mostrar la vista con gráficos
        public IActionResult IndexGraficos()
        {
            var dispositivos = ObtenerActividadDispositivos(); // Obtener datos iniciales
            var historialPings = ObtenerHistorialPings(); // Obtener historial de pings
            ViewBag.HistorialPings = historialPings; // Pasar historial a la vista
            return View("IndexGraficos", dispositivos); // Pasar datos a la vista
        }

        // Funcion para obtener el historial de pings atravez de AJAX
        public JsonResult ObtenerHistorialPingsAJAX(string ip)
        {
            // Obtener el historial actualizado de pings
            var historialPings = ObtenerHistorialPingsUltimos(ip);
            return Json(historialPings);
        }

        // Funciones para obtener toda la actividad de los dispositivos con el tiempo
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

        //Funcion para obatener el historial de todos los ping
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


        //Funcion para obatener el historial de todos los ping
        private List<Actividad> ObtenerHistorialPingsUltimos(string ip)
        {
            List<Actividad> historialPings = new List<Actividad>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerHistorialPingsUltimos]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@ip", ip);

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

        // Funcion para mostrar la vista de historial
        [HttpGet]
        public JsonResult ObtenerHistorialPingsAJAXParaIndexHistorial(int pageNumber = 1, int pageSize = 13)
        {
            try
            {
                // Llamamos a la función que obtiene datos paginados
                int totalRecords;
                var historialPings = ObtenerHistorialPingsParaIndexHistorial(pageNumber, pageSize, out totalRecords);

                // Validamos si el resultado es nulo o vacío
                if (historialPings == null || historialPings.Count == 0)
                {
                    return Json(new
                    {
                        Data = new List<Actividad>(),
                        TotalRecords = totalRecords,
                        CurrentPage = pageNumber
                    });
                }

                // Retornamos datos con JSON bien estructurado
                return Json(new
                {
                    Data = historialPings,
                    TotalRecords = totalRecords,
                    CurrentPage = pageNumber
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Error = "Error al obtener el historial de pings: " + ex.Message
                });
            }
        }


        // Funcion para mostrar la vista inicial
        public IActionResult Index(int pageNumber = 1, int pageSize = 13)
        {
            try
            {
                int totalRecords;
                var historialPings = ObtenerHistorialPingsParaIndexHistorial(pageNumber, pageSize, out totalRecords);

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRecords = totalRecords;
                ViewBag.HistorialPings = historialPings;

                return View(historialPings);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener el historial de pings: " + ex.Message;
                return View(new List<Actividad>());
            }
        }

        // funcion para obtener el historial de pings para pasarlo a la vista inicial
        private List<Actividad> ObtenerHistorialPingsParaIndexHistorial(int pageNumber, int pageSize, out int totalRecords)
        {
            List<Actividad> historialPings = new List<Actividad>();
            totalRecords = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("[dbo].[P_ObtenerHistorialPingsParaIndexHistorial]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

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
                        if (reader.NextResult() && reader.Read())
                        {
                            totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                        }
                    }
                }
            }

            return historialPings;
        }


    }
}
