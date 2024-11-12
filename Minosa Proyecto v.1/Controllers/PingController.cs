/*


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class ActividadController : Controller
    {
        private readonly IConfiguration _configuration;

        public ActividadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "Connection string is not configured properly.";
                return View();
            }

            List<Actividad> equiposList = new List<Actividad>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("P_ObtenerActividad", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var equipo = new Actividad
                        {
                            ID_Equipo = reader.GetInt32(reader.GetOrdinal("ID_equipo")),
                            DireccionIP = reader.GetString(reader.GetOrdinal("DireccionIP")),
                            Area = reader.GetString(reader.GetOrdinal("Area")),
                            DescripcionEquipo = reader.GetString(reader.GetOrdinal("DescripcionEquipo")),
                            TipoEquipo = reader.GetString(reader.GetOrdinal("TipoEquipo")),
                            Ping = reader.GetBoolean(reader.GetOrdinal("Ping"))
                        };

                        equiposList.Add(equipo);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error retrieving data: " + ex.Message;
                return View();
            }

            return View(equiposList);
        }



    }
}
*/