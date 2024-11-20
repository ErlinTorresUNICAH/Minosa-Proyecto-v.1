using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class MarcasController : Controller
    {
        private readonly IConfiguration _configuration;

        public MarcasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // CRUD MARCAS

        // Funcion para obtener todas las marcas
        public IActionResult Index()
        {
            List<Marca> marcas = new List<Marca>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerMarcas", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    marcas.Add(new Marca
                    {
                        ID_marca = (int)reader["ID_marca"],
                        NombreMarca = reader["Marca"].ToString(),
                        Activa = (bool)reader["Activa"]
                    });
                }
            }

            return View(marcas);
        }



        // Funcion Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        // Funcion Crear (POST)
        [HttpPost]
        public IActionResult Crear(Marca marca)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearMarca", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Marca", marca.NombreMarca);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }




        // Funcion Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Marca marca = new Marca();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerMarcaPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_marca", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    marca.ID_marca = (int)reader["ID_marca"];
                    marca.NombreMarca = reader["Marca"].ToString();
                    marca.Activa = (bool)reader["Activa"];
                }
            }

            return View(marca);
        }
        // Funcion Editar (POST)
        [HttpPost]
        public IActionResult Editar(Marca marca)
        {
            Console.WriteLine($"ID_marca: {marca.ID_marca}, NombreMarca: {marca.NombreMarca}, Activa: {marca.Activa}");

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarMarca", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_Marca", marca.ID_marca);
                command.Parameters.AddWithValue("@Marca", marca.NombreMarca);
                command.Parameters.AddWithValue("@Activa", marca.Activa);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }




        // Funcion Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Marca marca = new Marca();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerMarcaPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_marca", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    marca.ID_marca = (int)reader["ID_marca"];
                    marca.NombreMarca = reader["Marca"].ToString();
                }
            }

            return View(marca);
        }

        // Funcion Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(Marca marca)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_EliminarMarca", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_marca", marca.ID_marca);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado por si se trata de eliminar una marca asociada con una llave
            catch (SqlException ex)
            {
                if (ex.Number == 50000)
                {
                    ViewBag.Error = ex.Message;
                    return View(marca);
                }
            }

            return RedirectToAction("Index");
        }
    }
}