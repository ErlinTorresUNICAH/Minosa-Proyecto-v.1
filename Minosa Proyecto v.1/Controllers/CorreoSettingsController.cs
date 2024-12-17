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
    public class CorreoSettingsController : Controller
    {
        private readonly IConfiguration _configuration;

        public CorreoSettingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Muestra la configuración actual
        [HttpGet]
        public IActionResult Index()
        {
            var settings = ObtenerCorreoSettings();
            Console.WriteLine("index : ",settings);
            if (settings == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo cargar la configuración de correo.");
                return View(new CorreoSettings());
            }
            return View(settings);
        }

        // Muestra el formulario para editar
        [HttpGet]
        public IActionResult Editar()
        {
            var settings = ObtenerCorreoSettings();
            Console.WriteLine("index : " ,settings);
            if (settings == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo cargar la configuración de correo.");
                return RedirectToAction("Index");
            }
            return View(settings);
        }

        // Guarda los cambios en la configuración
        [HttpPost]
        public IActionResult Editar(CorreoSettings settings)
        {
            if (ModelState.IsValid)
            {
                ActualizarCorreoSettings(settings);
                return RedirectToAction("Index");
            }
            return View(settings);
        }

        // Método para obtener la configuración de correo desde la base de datos
        private CorreoSettings ObtenerCorreoSettings()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
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

        // Método para actualizar la configuración de correo en la base de datos
        private void ActualizarCorreoSettings(CorreoSettings settings)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("P_ActualizarCorreoSettings", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SmtpServer", settings.SmtpServer);
                        command.Parameters.AddWithValue("@SmtpPort", settings.SmtpPort);
                        command.Parameters.AddWithValue("@EmailFrom", settings.EmailFrom);
                        command.Parameters.AddWithValue("@EmailPassword", settings.EmailPassword);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}