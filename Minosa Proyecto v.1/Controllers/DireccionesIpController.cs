using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class DireccionesIpController : Controller
    {
        private readonly IConfiguration _configuration;

        public DireccionesIpController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Funcion/Lista para obtener las VLANs y pasarlas a las vistas
        private List<SelectListItem> ObtenerVlans()
        {
            List<SelectListItem> vlans = new List<SelectListItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_ObtenerVlanNombreDeVlans", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    vlans.Add(new SelectListItem
                    {
                        Value = reader["ID_vlan"].ToString(),
                        Text = reader["Nombre_Vlan"].ToString()
                    });
                }
            }

            return vlans;
        }

        // Acción para obtener todas las direcciones IP
        public IActionResult Index()
        {
            List<DireccionIp> direcciones = new List<DireccionIp>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDireccionesIp", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    direcciones.Add(new DireccionIp
                    {
                        ID_ip = (int)reader["ID_ip"],
                        IPV4 = reader["IPV4"].ToString(),
                        Estado = reader["Estado"].ToString(),
                        Nombre_Vlan = reader["Nombre_Vlan"].ToString(),
                        Activa = (bool)reader["Activa"],
                        ping = (bool)reader["ping"]
                    });
                }
            }

            return View(direcciones);
        }



        //CRUD VLANS

        // Funcion Leer
        // El Leer es el Index



        // Funcion Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Vlans = ObtenerVlans();
            return View();
        }
        // Funcion Crear (POST)
        [HttpPost]
        public IActionResult Crear(DireccionIp direccion)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearDireccionIp", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IPV4", direccion.IPV4);
                command.Parameters.AddWithValue("@Estado", direccion.Estado);
                command.Parameters.AddWithValue("@id_vlan", direccion.id_vlan);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }



        // Funcion Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            DireccionIp direccion = new DireccionIp();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDireccionIpPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_ip", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    direccion.ID_ip = (int)reader["ID_ip"];
                    direccion.IPV4 = reader["IPV4"].ToString();
                    direccion.Estado = reader["Estado"].ToString();
                    direccion.id_vlan = (int)reader["id_vlan"];
                    direccion.Activa = (bool)reader["Activa"];
                }
            }

            ViewBag.Vlans = ObtenerVlans();
            return View(direccion);
        }
        // Funcion Editar (POST)
        [HttpPost]
        public IActionResult Editar(DireccionIp direccion)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarDireccionIp", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_ip", direccion.ID_ip);
                command.Parameters.AddWithValue("@IPV4", direccion.IPV4);
                command.Parameters.AddWithValue("@Estado", direccion.Estado);
                command.Parameters.AddWithValue("@id_vlan", direccion.id_vlan);
                command.Parameters.AddWithValue("@Activa", direccion.Activa);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }



        // Funcion Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            DireccionIp direccion = new DireccionIp();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerDireccionIpPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_ip", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    direccion.ID_ip = (int)reader["ID_ip"];
                    direccion.IPV4 = reader["IPV4"].ToString();
                    direccion.Estado = reader["Estado"].ToString();
                    direccion.Nombre_Vlan = reader["Nombre_Vlan"].ToString();
                }
            }

            return View(direccion);
        }
        // Funcion Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(DireccionIp direccion)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_EliminarDireccionIp", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_ip", direccion.ID_ip);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado, para que no tenga problemas si se intenta eliminar una IP con llave asignada
            catch (SqlException ex)
            {
                if (ex.Number >= 50000)  
                {
                    ViewBag.Error = ex.Message;
                    return View(direccion);
                }
                throw;
            }

            return RedirectToAction("Index");
        }
        // Funcion Eliminar Multiples Ips (POST)
        [HttpPost]
        public IActionResult EliminarMultiples(string selectedIds)
        {
            if (!string.IsNullOrEmpty(selectedIds))
            {
                int[] idsToDelete = selectedIds.Split(',').Select(int.Parse).ToArray();

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                List<string> errorMessages = new List<string>();

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        foreach (var id in idsToDelete)
                        {
                            using (var command = new SqlCommand("P_GRUD_EliminarDireccionIp", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ID_ip", id);

                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    errorMessages.Add($"Error deleting IP with ID {id}: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error: {ex.Message}";
                    return RedirectToAction("Index");
                }

                if (errorMessages.Any())
                {
                    ViewBag.Error = string.Join("<br/>", errorMessages);
                }
            }
            return RedirectToAction("Index");
        }




    }
}