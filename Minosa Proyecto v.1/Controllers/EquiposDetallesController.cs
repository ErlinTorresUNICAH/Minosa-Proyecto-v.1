using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Minosa_Proyecto_v._1.Controllers
{   //CRUD - READ - CREATE - UPDATE - DELETE
    public class EquiposDetallesController : Controller
    {
        private readonly IConfiguration _configuration;

        public EquiposDetallesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Detalles


        //Crear select box
        [HttpGet]
        public IActionResult Crear()
        {
            var model = new EquipoDetalleViewModel();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Cargar Proveedores, Modelos, Tipos de Equipos, Áreas e IPs
                model.Proveedores = ObtenerSelectList(connection, "P_ObtenerProveedores", "ID_proveedor", "Nombre");
                model.Modelos = ObtenerSelectList(connection, "P_ObtenerModelos", "ID_modelo", "Nombre_Modelo");
                model.TiposEquipos = ObtenerSelectList(connection, "P_ObtenerTiposEquipos", "ID_tipo_equipo", "Tipo_Equipo");
                model.Areas = ObtenerSelectList(connection, "P_ObtenerAreas", "ID_area", "Nombre_Area");
                model.IPs = ObtenerSelectList(connection, "P_ObtenerIPs", "ID_ip", "IPV4");
            }

            return View(model);
        }

        //Crear
        [HttpPost]
        public IActionResult Crear(EquipoDetalleViewModel model)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("P_InsertarEquipoCompleto", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar los parámetros
                    command.Parameters.AddWithValue("@NumeroSerie", model.NumeroSerie);
                    command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
                    command.Parameters.AddWithValue("@id_tipo_equipo", model.ID_tipo_equipo);
                    command.Parameters.AddWithValue("@id_modelo", model.ID_modelo);
                    command.Parameters.AddWithValue ("@id_area", model.ID_area);
                    command.Parameters.AddWithValue("@id_ip", model.ID_ip);
                    command.Parameters.AddWithValue("@Estado", model.Estado);
                    command.Parameters.AddWithValue("@Activo", model.Activo);
                    command.Parameters.AddWithValue("@Respaldo", model.Respaldo);
                    command.Parameters.AddWithValue("@Observaciones", model.Observaciones);
                    command.Parameters.AddWithValue("@Tipo_Voltaje", model.Tipo_Voltaje);
                    command.Parameters.AddWithValue("@Voltaje", model.Voltaje);
                    command.Parameters.AddWithValue("@Amperaje", model.Amperaje);
                    command.Parameters.AddWithValue("@Num_Puertos_RJ45", model.Num_Puertos_RJ45);
                    command.Parameters.AddWithValue("@Num_Puertos_SFP", model.Num_Puertos_SFP);
                    command.Parameters.AddWithValue("@Fecha_Compra", model.Fecha_Compra);
                    command.Parameters.AddWithValue("@Fecha_Garantia", model.Fecha_Garantia);
                    command.Parameters.AddWithValue("@Tipo_Garantia", model.Tipo_Garantia);
                    command.Parameters.AddWithValue("@Canal", model.Canal);
                    command.Parameters.AddWithValue("@Firmware", model.Firmware);
                    command.Parameters.AddWithValue("@Usuario", model.Usuario);
                    command.Parameters.AddWithValue("@Contracena", model.Contracena);
                    command.Parameters.AddWithValue("@MAC_Address", model.MAC_Address);
                    command.Parameters.AddWithValue("@Fecha_Instalacion", model.Fecha_Instalacion);
                    command.Parameters.AddWithValue("@Ultima_Actualizacion", model.Ultima_Actualizacion);
                    command.Parameters.AddWithValue("@Voltaje_Energia", model.Voltaje_Energia);
                    command.Parameters.AddWithValue("@id_proveedor", model.ID_proveedor);

                    command.ExecuteNonQuery();
                    return RedirectToAction("Index", "Equipos");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        private List<SelectListItem> ObtenerSelectList(SqlConnection connection, string storedProcedure, string valueField, string textField)
        {
            var selectList = new List<SelectListItem>();
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = reader[valueField].ToString(),
                            Text = reader[textField].ToString()
                        });
                    }
                }
            }
            return selectList;
        }


        [HttpPost]
        public IActionResult Editar(EquipoDetalleViewModel model)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("P_ActualizarEquipoDetalle", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Add parameters
                    command.Parameters.AddWithValue("@ID_equipo", model.ID_equipo);
                    command.Parameters.AddWithValue("@NumeroSerie", model.NumeroSerie);
                    command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
                    command.Parameters.AddWithValue("@id_tipo_equipo", model.ID_tipo_equipo);
                    command.Parameters.AddWithValue("@id_modelo", model.ID_modelo);
                    command.Parameters.AddWithValue("@id_area", model.ID_area);
                    command.Parameters.AddWithValue("@id_ip", model.ID_ip);
                    command.Parameters.AddWithValue("@Estado", model.Estado);
                    command.Parameters.AddWithValue("@Activo", model.Activo);
                    command.Parameters.AddWithValue("@Respaldo", model.Respaldo);
                    command.Parameters.AddWithValue("@Observaciones", model.Observaciones);
                    command.Parameters.AddWithValue("@Tipo_Voltaje", model.Tipo_Voltaje);
                    command.Parameters.AddWithValue("@Voltaje", model.Voltaje);
                    command.Parameters.AddWithValue("@Amperaje", model.Amperaje);
                    command.Parameters.AddWithValue("@Num_Puertos_RJ45", model.Num_Puertos_RJ45);
                    command.Parameters.AddWithValue("@Num_Puertos_SFP", model.Num_Puertos_SFP);
                    command.Parameters.AddWithValue("@Fecha_Compra", model.Fecha_Compra);
                    command.Parameters.AddWithValue("@Fecha_Garantia", model.Fecha_Garantia);
                    command.Parameters.AddWithValue("@Tipo_Garantia", model.Tipo_Garantia);
                    command.Parameters.AddWithValue("@Canal", model.Canal);
                    command.Parameters.AddWithValue("@Firmware", model.Firmware);
                    command.Parameters.AddWithValue("@Usuario", model.Usuario);
                    command.Parameters.AddWithValue("@Contracena", model.Contracena);
                    command.Parameters.AddWithValue("@MAC_Address", model.MAC_Address);
                    command.Parameters.AddWithValue("@Fecha_Instalacion", model.Fecha_Instalacion);
                    command.Parameters.AddWithValue("@Ultima_Actualizacion", model.Ultima_Actualizacion);
                    command.Parameters.AddWithValue("@Voltaje_Energia", model.Voltaje_Energia);
                    command.Parameters.AddWithValue("@id_proveedor", model.ID_proveedor);

                    command.ExecuteNonQuery();
                    return RedirectToAction("Detalles", new { id = model.ID_equipo });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }


        }


        [HttpGet]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var model = new EquipoDetalleViewModel();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Llamada al procedimiento almacenado para obtener los detalles del equipo
                var command = new SqlCommand("P_ObtenerEquipoDetalle", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ID_equipo", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Asignación de los datos del equipo al modelo
                        model.ID_equipo = id;
                        model.NumeroSerie = reader["NumeroSerie"]?.ToString();
                        model.Descripcion = reader["Descripcion"]?.ToString();
                        model.Estado = reader["Estado"]?.ToString();
                        model.Activo = (bool)reader["Activo"];
                        model.Respaldo = reader["Respaldo"]?.ToString();
                        model.Observaciones = reader["Observaciones"]?.ToString();
                        model.Tipo_Voltaje = reader["Tipo_Voltaje"]?.ToString();
                        model.Voltaje = Convert.ToInt32(reader["Voltaje"]);
                        model.Amperaje = Convert.ToInt32(reader["Amperaje"]);
                        model.Num_Puertos_RJ45 = Convert.ToInt32(reader["Num_Puertos_RJ45"]);
                        model.Num_Puertos_SFP = Convert.ToInt32(reader["Num_Puertos_SFP"]);
                        model.Fecha_Compra = Convert.ToDateTime(reader["Fecha_Compra"]);
                        model.Fecha_Garantia = Convert.ToDateTime(reader["Fecha_Garantia"]);
                        model.Tipo_Garantia = reader["Tipo_Garantia"]?.ToString();
                        model.Canal = reader["Canal"]?.ToString();
                        model.Firmware = reader["Firmware"]?.ToString();
                        model.Usuario = reader["Usuario"]?.ToString();
                        model.Contracena = reader["Contracena"]?.ToString();
                        model.MAC_Address = reader["MAC_Address"]?.ToString();
                        model.Fecha_Instalacion = reader["Fecha_Instalacion"]?.ToString();
                        model.Ultima_Actualizacion = reader["Ultima_Actualizacion"]?.ToString();
                        model.Voltaje_Energia = reader["Voltaje_Energia"]?.ToString();
                        model.ID_proveedor = Convert.ToInt32(reader["ID_proveedor"]);
                        model.ID_modelo = Convert.ToInt32(reader["ID_modelo"]);
                        model.ID_tipo_equipo = Convert.ToInt32(reader["ID_tipo_equipo"]);
                        model.ID_area = Convert.ToInt32(reader["ID_area"]);
                        model.ID_ip = Convert.ToInt32(reader["ID_ip"]);
                        model.Nombre_Proveedor = reader["Nombre_Proveedor"]?.ToString();
                        model.Direccion_Proveedor = reader["Direccion_Proveedor"]?.ToString();
                        model.Telefono_Proveedor = reader["Telefono_Proveedor"]?.ToString();
                        model.Correo_Proveedor = reader["Correo_Proveedor"]?.ToString();
                        model.Nombre_Modelo = reader["Nombre_Modelo"]?.ToString();
                        model.Nombre_Area = reader["Nombre_Area"]?.ToString();
                        model.Direccion_IP = reader["Direccion_IP"]?.ToString();
                        model.Tipo_Equipo = reader["Tipo_Equipo"]?.ToString();
                    }
                }

                // Cargar los SelectList para la vista de edición
                model.Proveedores = ObtenerSelectList(connection, "P_ObtenerProveedores", "ID_proveedor", "Nombre");
                model.Modelos = ObtenerSelectList(connection, "P_ObtenerModelos", "ID_modelo", "Nombre_Modelo");
                model.TiposEquipos = ObtenerSelectList(connection, "P_ObtenerTiposEquipos", "ID_tipo_equipo", "Tipo_Equipo");
                model.Areas = ObtenerSelectList(connection, "P_ObtenerAreas", "ID_area", "Nombre_Area");
                model.IPs = ObtenerSelectList(connection, "P_ObtenerIPs", "ID_ip", "IPV4");
            }

            return View(model);
        }




        //Read
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var model = new EquipoDetalleViewModel();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var command = new SqlCommand("P_ObtenerEquipoDetalle", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_equipo", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.ID_equipo = id;
                        model.NumeroSerie = reader["NumeroSerie"].ToString();
                        model.Descripcion = reader["Descripcion"].ToString();
                        model.Estado = reader["Estado"].ToString();
                        model.Activo = (bool)reader["Activo"];
                        model.Respaldo = reader["Respaldo"].ToString();
                        model.Observaciones = reader["Observaciones"].ToString();
                        model.ID_detalle_equipo = (int)reader["ID_detalle_equipo"];
                        model.Tipo_Voltaje = reader["Tipo_Voltaje"].ToString();
                        model.Voltaje = (int)reader["Voltaje"];
                        model.Amperaje = (int)reader["Amperaje"];
                        model.Num_Puertos_RJ45 = (int)reader["Num_Puertos_RJ45"];
                        model.Num_Puertos_SFP = (int)reader["Num_Puertos_SFP"];
                        model.Fecha_Compra = (DateTime)reader["Fecha_Compra"];
                        model.Fecha_Garantia = (DateTime)reader["Fecha_Garantia"];
                        model.Tipo_Garantia = reader["Tipo_Garantia"].ToString();
                        model.Canal = reader["Canal"].ToString();
                        model.Firmware = reader["Firmware"].ToString();
                        model.Usuario = reader["Usuario"].ToString();
                        model.Contracena = reader["Contracena"].ToString();
                        model.MAC_Address = reader["MAC_Address"].ToString();
                        model.Fecha_Instalacion = reader["Fecha_Instalacion"].ToString();
                        model.Ultima_Actualizacion = reader["Ultima_Actualizacion"].ToString();
                        model.Voltaje_Energia = reader["Voltaje_Energia"].ToString();
                        model.ID_proveedor = (int)reader["ID_proveedor"];
                        model.ID_modelo = (int)reader["ID_modelo"];
                        model.ID_tipo_equipo = (int)reader["ID_tipo_equipo"];
                        model.ID_area = (int)reader["ID_area"];
                        model.ID_ip = (int)reader["ID_ip"];
                        model.Nombre_Proveedor = reader["Nombre_Proveedor"].ToString();
                        model.Direccion_Proveedor = reader["Direccion_Proveedor"].ToString();
                        model.Telefono_Proveedor = reader["Telefono_Proveedor"].ToString();
                        model.Correo_Proveedor = reader["Correo_Proveedor"].ToString();
                        model.Nombre_Modelo = reader["Nombre_Modelo"].ToString();
                        model.Nombre_Area = reader["Nombre_Area"].ToString();
                        model.Direccion_IP = reader["Direccion_IP"].ToString();
                        model.Tipo_Equipo = reader["Tipo_Equipo"].ToString();
                    }
                }
            }

            return View(model);
        }





        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var model = new EquipoDetalleViewModel();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Obtener los detalles del equipo para mostrar en la vista
                var command = new SqlCommand("P_ObtenerEquipoDetalle", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ID_equipo", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.ID_equipo = id;
                        model.NumeroSerie = reader["NumeroSerie"]?.ToString();
                        model.Descripcion = reader["Descripcion"]?.ToString();
                        model.Estado = reader["Estado"]?.ToString();
                        model.Nombre_Area = reader["Nombre_Area"]?.ToString();
                        model.Tipo_Equipo = reader["Tipo_Equipo"]?.ToString();
                    }
                    else
                    {
                        // Si el equipo no existe, redirige al índice con un mensaje de error.
                        TempData["Error"] = "El equipo no existe o ya fue eliminado.";
                        return RedirectToAction("Index", "Equipos");
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        public IActionResult ConfirmarEliminacion(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Llamar al procedimiento almacenado para eliminar equipo y sus detalles
                            var deleteCommand = new SqlCommand("P_EliminarEquipoCompleto", connection, transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            deleteCommand.Parameters.AddWithValue("@ID_equipo", id);
                            deleteCommand.ExecuteNonQuery();

                            // Confirmar la transacción
                            transaction.Commit();
                        }
                        catch
                        {
                            // Revertir en caso de error
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                TempData["Success"] = "Equipo eliminado exitosamente.";
                return RedirectToAction("Index", "Equipos");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al eliminar el equipo: {ex.Message}";
                return View();
            }
        }


    }
}
