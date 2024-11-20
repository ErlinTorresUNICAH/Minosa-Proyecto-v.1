using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class AreasController : Controller
    {
        private readonly IConfiguration _configuration;

        public AreasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Funcion/Lista para obtener las zonas y pasarlas a las vistas
        private List<SelectListItem> ObtenerZonas()
        {
            List<SelectListItem> zonas = new List<SelectListItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("ObtenerNombreIdDeZona", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    zonas.Add(new SelectListItem
                    {
                        Value = reader["ID_zona"].ToString(),
                        Text = reader["Nombre_Zona"].ToString()
                    });
                }
            }

            return zonas;
        }

        // Funcion para obtener todas las áreas
        public IActionResult Index()
        {
            List<Area> areas = new List<Area>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerAreas", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    areas.Add(new Area
                    {
                        ID_area = (int)reader["ID_area"],
                        Nombre_Area = reader["Nombre_Area"].ToString(),
                        Nombre_Zona = reader["Nombre_Zona"].ToString(),
                        Activo = (bool)reader["Activo"]
                    });
                }
            }

            return View(areas);
        }


        // CRUD AREAS

        // Funcion Leer
        // El Leer es el Index



        // Funcion Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Zonas = ObtenerZonas();
            return View();
        }
        // Funcion Crear (POST)
        [HttpPost]
        public IActionResult Crear(Area area)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearArea", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Area", area.Nombre_Area);
                command.Parameters.AddWithValue("@id_zona", area.id_zona);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }



        // Funcion Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Area area = new Area();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerAreaPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_area", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    area.ID_area = (int)reader["ID_area"];
                    area.Nombre_Area = reader["Nombre_Area"].ToString();
                    area.id_zona = (int)reader["id_zona"];
                    area.Activo = (bool)reader["Activo"];
                }
            }

            ViewBag.Zonas = ObtenerZonas();
            return View(area);
        }
        // Funcion Editar (POST)
        [HttpPost]
        public IActionResult Editar(Area area)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarArea", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_area", area.ID_area);
                command.Parameters.AddWithValue("@Nombre_Area", area.Nombre_Area);
                command.Parameters.AddWithValue("@id_zona", area.id_zona);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }



        // Funcion Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Area area = new Area();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerAreaPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_area", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    area.ID_area = (int)reader["ID_area"];
                    area.Nombre_Area = reader["Nombre_Area"].ToString();
                    area.Nombre_Zona = reader["Nombre_Zona"].ToString();
                }
            }

            return View(area);
        }
        // Funcions Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(Area area)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_EliminarArea", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_area", area.ID_area);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado, en caso de tenga una zona asiganada y no tenga problemas con las llaves
            catch (SqlException ex)
            {
                if (ex.Number == 50000)
                {
                    ViewBag.Error = ex.Message;
                    return View(area);
                }
            }

            return RedirectToAction("Index");
        }
    }
}