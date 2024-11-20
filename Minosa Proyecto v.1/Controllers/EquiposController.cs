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
    public class EquiposController : Controller
    {
        private readonly IConfiguration _configuration;

        public EquiposController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Leer Equipo, Para la visualiacion de los equipos se creo un controllador y una vista.
        [HttpGet]
        public IActionResult Index()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "La conexion no esta configurada correctamente";
                return View();
            }

            List<EquipoViewModel> equiposList = new List<EquipoViewModel>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("P_ObtenerDatosEquipos", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var equipo = new EquipoViewModel
                        {
                            ID_equipo = reader.GetInt32(reader.GetOrdinal("ID_equipo")),
                            NumeroSerie = reader.GetString(reader.GetOrdinal("NumeroSerie")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Tipo_Equipo = reader.GetString(reader.GetOrdinal("Tipo_Equipo")),
                            Modelo = reader.GetString(reader.GetOrdinal("Modelo")),
                            Area = reader.GetString(reader.GetOrdinal("Area")),
                            Direccion_IP = reader.GetString(reader.GetOrdinal("Direccion_IP")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            Respaldo = reader.IsDBNull(reader.GetOrdinal("Respaldo")) ? null : reader.GetString(reader.GetOrdinal("Respaldo")),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones"))
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
