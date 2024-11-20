using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class TipoEquiposController : Controller
    {
        private readonly IConfiguration _configuration;

        public TipoEquiposController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // Funcion que obtiene los tipos de equipos
        public IActionResult Index()
        {
            List<TipoEquipo> tiposEquipos = new List<TipoEquipo>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerTiposEquipos", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tiposEquipos.Add(new TipoEquipo
                    {
                        ID_tipo_equipo = (int)reader["ID_tipo_equipo"],
                        Tipo_Equipo = reader["Tipo_Equipo"].ToString(),
                        Creacion_Tipo_Equipo = (DateTime)reader["Creacion_Tipo_Equipo"]
                    });
                }
            }
            return View(tiposEquipos);
        }



        // CRUD Tipo de Equipo

        // Leer Tipo de equipo
        // La funcion es Index


        // Funcion Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        // Funcion Crear (POST)
        [HttpPost]
        public IActionResult Crear(TipoEquipo tipoEquipo)
        {
            if (string.IsNullOrEmpty(tipoEquipo.Tipo_Equipo))
            {
                ViewBag.Error = "El tipo de equipo es obligatorio.";
                return View(tipoEquipo);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_CrearTipoEquipo", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Tipo_Equipo", tipoEquipo.Tipo_Equipo);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado por si se trata de insertar un tipo de equipo incorrecto
            catch (SqlException ex)
            {
                if (ex.Number >= 50000)  
                {
                    ViewBag.Error = ex.Message;
                }
                throw;
            }
            return RedirectToAction("Index");
        }



        // Funcion Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            TipoEquipo tipoEquipo = new TipoEquipo();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerTipoEquipoPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_TipoEquipo", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tipoEquipo.ID_tipo_equipo = (int)reader["ID_tipo_equipo"];
                    tipoEquipo.Tipo_Equipo = reader["Tipo_Equipo"].ToString();
                    tipoEquipo.Creacion_Tipo_Equipo = (DateTime)reader["Creacion_Tipo_Equipo"];
                }
            }
            return View(tipoEquipo);
        }
        // Funcion Editar (POST)
        [HttpPost]
        public IActionResult Editar(TipoEquipo tipoEquipo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarTipoEquipo", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_TipoEquipo", tipoEquipo.ID_tipo_equipo);
                command.Parameters.AddWithValue("@Tipo_Equipo", tipoEquipo.Tipo_Equipo);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }



        // Funcion Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            TipoEquipo tipoEquipo = new TipoEquipo();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerTipoEquipoPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_TipoEquipo", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tipoEquipo.ID_tipo_equipo = (int)reader["ID_tipo_equipo"];
                    tipoEquipo.Tipo_Equipo = reader["Tipo_Equipo"].ToString();
                }
            }
            return View(tipoEquipo);
        }
        // Funcion Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(TipoEquipo tipoEquipo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_EliminarTipoEquipo", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_TipoEquipo", tipoEquipo.ID_tipo_equipo);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado por si se trata de eliminar un tipo de equipo que esta asignado a llave
            catch (SqlException ex)
            {
                if (ex.Number >= 50000)  
                {
                    ViewBag.Error = ex.Message;
                    return View(tipoEquipo);
                }
                throw;
            }
            return RedirectToAction("Index");
        }
    }
}

