using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize] // Asegúrate de que solo usuarios autenticados accedan a esta acción
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Detalles()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            ViewBag.Error = null;  // Inicializamos ViewBag.Error para evitar el null reference

            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "La cadena de conexión no está configurada correctamente.";
                return View();
            }

            UsuarioViewModel usuario = null;

            // Obtener el ID del usuario logueado desde las claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                ViewBag.Error = "El ID del usuario no se encontró.";
                return View();
            }

            int userId = int.Parse(userIdClaim.Value); // Convierte el ID del usuario a entero

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Usamos el procedimiento almacenado P_ObtenerUsuarioPorID
                    var command = new SqlCommand("P_ObtenerUsuarioPorID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID_usuario", userId)); // Pasamos el ID del usuario

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        usuario = new UsuarioViewModel
                        {
                            ID_usuario = reader.GetInt32(reader.GetOrdinal("ID_usuario")),
                            Nombre_Usuario = reader.GetString(reader.GetOrdinal("Nombre_Usuario")),
                            Contrasena = reader.GetString(reader.GetOrdinal("Contrasena")),
                            Nombre_Rol = reader.GetString(reader.GetOrdinal("Nombre_Rol")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al obtener los datos: " + ex.Message;
                return View();
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioViewModel model)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "La cadena de conexión no está configurada correctamente.";
                return View(model);
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("P_ActualizarUsuario", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros para actualizar
                    command.Parameters.Add(new SqlParameter("@ID_usuario", model.ID_usuario));
                    command.Parameters.Add(new SqlParameter("@Nombre_Usuario", model.Nombre_Usuario));
                    command.Parameters.Add(new SqlParameter("@Contrasena", model.Contrasena));

                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Detalles"); // Regresar a la página de detalles después de la edición
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al actualizar los datos: " + ex.Message;
                return View(model);
            }
        }
    }
}
