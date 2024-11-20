using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class ModelosController : Controller
    {
        private readonly IConfiguration _configuration;

        public ModelosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lista para obtener las marcas y pasarlas a las vistas
        private List<SelectListItem> ObtenerMarcas()
        {
            List<SelectListItem> marcas = new List<SelectListItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    marcas.Add(new SelectListItem
                    {
                        Value = reader["ID_marca"].ToString(),
                        Text = reader["Marca"].ToString()
                    });
                }
            }

            return marcas;
        }

        // Acción para obtener todos los modelos
        public IActionResult Index()
        {
            List<Modelo> modelos = new List<Modelo>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerModelos", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    modelos.Add(new Modelo
                    {
                        ID_modelo = (int)reader["ID_modelo"],
                        Nombre_Modelo = reader["Nombre_Modelo"].ToString(),
                        Marca = reader["Marca"].ToString(),
                        Activo = (bool)reader["Activo"]
                    });
                }
            }

            return View(modelos);
        }


        //CRUD Modelo


        // Funcion Leer
        // La funcion leeer es el Index



        // Acción Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Marcas = ObtenerMarcas();
            return View();
        }
        // Acción Crear (POST)
        [HttpPost]
        public IActionResult Crear(Modelo modelo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearModelo", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Modelo", modelo.Nombre_Modelo);
                command.Parameters.AddWithValue("@id_Marca", modelo.id_Marca);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }




        // Acción Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Modelo modelo = new Modelo();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerModeloPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_modelo", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    modelo.ID_modelo = (int)reader["ID_modelo"];
                    modelo.Nombre_Modelo = reader["Nombre_Modelo"].ToString();
                    modelo.id_Marca = (int)reader["id_Marca"];
                    modelo.Activo = (bool)reader["Activo"];
                }
            }

            ViewBag.Marcas = ObtenerMarcas();
            return View(modelo);
        }
        // Acción Editar (POST)
        [HttpPost]
        public IActionResult Editar(Modelo modelo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_ActualizarModelo", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_modelo", modelo.ID_modelo);
                    command.Parameters.AddWithValue("@Nombre_Modelo", modelo.Nombre_Modelo);
                    command.Parameters.AddWithValue("@id_Marca", modelo.id_Marca);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            return RedirectToAction("Index");
        }




        // Acción Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Modelo modelo = new Modelo();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerModeloPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_modelo", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    modelo.ID_modelo = (int)reader["ID_modelo"];
                    modelo.Nombre_Modelo = reader["Nombre_Modelo"].ToString();
                    modelo.Marca = reader["Marca"].ToString();
                }
            }

            return View(modelo);
        }
        // Acción Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(Modelo modelo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_EliminarModeloCompleto", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_modelo", modelo.ID_modelo);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado por si quiere eliminar un modelo que ya esta asignado a una llave
            catch (SqlException ex)
            {
                if (ex.Number == 50000)  
                {
                    ViewBag.Error = ex.Message;
                    return View(modelo);
                }
            }

            return RedirectToAction("Index");
        }
    }
}