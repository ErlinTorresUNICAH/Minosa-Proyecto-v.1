using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class DestinatariosController : Controller
    {
        private readonly IConfiguration _configuration;

        public DestinatariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Obtener la lista de alertas
        private List<SelectListItem> ObtenerAlertas()
        {
            List<SelectListItem> alertas = new List<SelectListItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT ID_alerta, Nombre_Alerta FROM Tipo_Alerta", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    alertas.Add(new SelectListItem
                    {
                        Value = reader["ID_alerta"].ToString(),
                        Text = reader["Nombre_Alerta"].ToString()
                    });
                }
            }

            return alertas;
        }

        // Listar Destinatarios
        public IActionResult Index()
        {
            List<Destinatario> destinatarios = new List<Destinatario>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDestinatarios", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    destinatarios.Add(new Destinatario
                    {
                        ID_destinatario = (int)reader["ID_destinatario"],
                        Nombre_Destinatario = reader["Nombre_Destinatario"].ToString(),
                        Correo_Destinatario = reader["Correo_Destinatario"].ToString(),
                        Descripcion_Destinatario = reader["Descripcion_Destinatario"].ToString(),
                        Nombre_Alerta = reader["Nombre_Alerta"].ToString()
                    });
                }
            }

            return View(destinatarios);
        }

        // Crear Destinatario (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Alertas = ObtenerAlertas();
            return View();
        }

        // Crear Destinatario (POST)
        [HttpPost]
        public IActionResult Crear(Destinatario destinatario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearDestinatario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Destinatario", destinatario.Nombre_Destinatario);
                command.Parameters.AddWithValue("@Correo_Destinatario", destinatario.Correo_Destinatario);
                command.Parameters.AddWithValue("@Descripcion_Destinatario", destinatario.Descripcion_Destinatario);
                command.Parameters.AddWithValue("@id_alerta", destinatario.id_alerta);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Editar Destinatario (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Destinatario destinatario = new Destinatario();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDestinatarioPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_destinatario", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    destinatario.ID_destinatario = (int)reader["ID_destinatario"];
                    destinatario.Nombre_Destinatario = reader["Nombre_Destinatario"].ToString();
                    destinatario.Correo_Destinatario = reader["Correo_Destinatario"].ToString();
                    destinatario.Descripcion_Destinatario = reader["Descripcion_Destinatario"].ToString();
                    destinatario.id_alerta = (int)reader["id_alerta"];
                }
            }

            ViewBag.Alertas = ObtenerAlertas();
            return View(destinatario);
        }

        // Editar Destinatario (POST)
        [HttpPost]
        public IActionResult Editar(Destinatario destinatario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarDestinatario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_destinatario", destinatario.ID_destinatario);
                command.Parameters.AddWithValue("@Nombre_Destinatario", destinatario.Nombre_Destinatario);
                command.Parameters.AddWithValue("@Correo_Destinatario", destinatario.Correo_Destinatario);
                command.Parameters.AddWithValue("@Descripcion_Destinatario", destinatario.Descripcion_Destinatario);
                command.Parameters.AddWithValue("@id_alerta", destinatario.id_alerta);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Eliminar Destinatario (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Destinatario destinatario = new Destinatario();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDestinatarioPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_destinatario", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    destinatario.ID_destinatario = (int)reader["ID_destinatario"];
                    destinatario.Nombre_Destinatario = reader["Nombre_Destinatario"].ToString();
                    destinatario.Correo_Destinatario = reader["Correo_Destinatario"].ToString();
                    destinatario.Descripcion_Destinatario = reader["Descripcion_Destinatario"].ToString();
                    destinatario.Nombre_Alerta = reader["id_alerta"].ToString();
                }
            }

            return View(destinatario);
        }

        // Eliminar Destinatario (POST)
        [HttpPost]
        public IActionResult Eliminar(Destinatario destinatario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_EliminarDestinatario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_destinatario", destinatario.ID_destinatario);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
