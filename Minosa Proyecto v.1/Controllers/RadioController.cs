using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Minosa_Proyecto_v._1.Models;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace Minosa_Proyecto_v._1.Controllers
{
    public class RadioController : Controller
    {
        private readonly IConfiguration _configuration;

        public RadioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: RadioController/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            var model = new Radio();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                model.Proveedores = ObtenerSelectList(connection, "P_ObtenerProveedores", "ID_proveedor", "Nombre");
                model.Modelos = ObtenerSelectList(connection, "P_ObtenerModelos", "ID_modelo", "Nombre_Modelo");
                model.TiposEquipos = ObtenerSelectList(connection, "P_ObtenerTiposEquipos", "ID_tipo_equipo", "Tipo_Equipo");
                model.Areas = ObtenerSelectList(connection, "P_ObtenerAreas", "ID_area", "Nombre_Area");
                model.IPs = ObtenerSelectList(connection, "P_ObtenerIPs", "ID_ip", "IPV4");
            }

            return View(model);
        }

        // POST: RadioController/Crear
        [HttpPost]
        public IActionResult Crear(Radio model)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.InfoMessage += Connection_InfoMessage;  // Escuchar mensajes
                    connection.Open();

                    // Obtener dinámicamente el ID del tipo de equipo 'Radio'
                    var tipoEquipoCommand = new SqlCommand("SELECT ID_tipo_equipo FROM Tipo_Equipos WHERE UPPER(Tipo_Equipo) = 'RADIO'", connection);
                    object? tipoEquipoRadioIdObj = tipoEquipoCommand.ExecuteScalar();

                    try { 

                        if (tipoEquipoRadioIdObj == null)
                        {
                        // No existe el tipo de equipo "Radio" en la base de datos
                        ModelState.AddModelError("", "Error: El tipo de equipo 'Radio' no está disponible.");

                     
                        }
                    }
                    catch (Exception ef)
                    {
                        ViewBag.Error = ef.Message;
                        return View(model);
                    }
                    int tipoEquipoRadioId = (int)tipoEquipoRadioIdObj;
                    
                    // Asegurar que el id_tipo_equipo proporcionado es de tipo 'Radio'
                    if (model.ID_tipo_equipo != tipoEquipoRadioId)
                    {
                        ModelState.AddModelError("", "Error: Solo se pueden crear dispositivos de tipo 'Radio'.");
                        
                    }


                    var command = new SqlCommand("P_InsertarEquipoYRadio", connection);


                    command.CommandType = CommandType.StoredProcedure;
                    
                    Console.WriteLine("hola");
                    // Parametros de equipo y detalles equipo
                    command.Parameters.AddWithValue("@NumeroSerie", model.NumeroSerie);
                    command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
                    command.Parameters.AddWithValue("@id_tipo_equipo", tipoEquipoRadioId); // ID dinámico para Radio
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

                    // Parametros de radio
                    command.Parameters.AddWithValue("@Frecuencia", model.Frecuencia);
                    command.Parameters.AddWithValue("@Frecuencia_Rango", model.Frecuencia_Rango);
                    command.Parameters.AddWithValue("@Modo", model.Modo);
                    command.Parameters.AddWithValue("@Ssid", model.Ssid);
                    command.Parameters.AddWithValue("@Modulacion", model.Modulacion);
                    command.Parameters.AddWithValue("@Potencia", model.Potencia);
                    command.Parameters.AddWithValue("@Tx_Power", model.Tx_Power);
                    command.Parameters.AddWithValue("@Rx_Level", model.Rx_Level);
                    command.Parameters.AddWithValue("@Tx_Freq", model.Tx_Freq);


                    Console.WriteLine("holafinal" + model.Descripcion);
                    Console.WriteLine("holafinal" + tipoEquipoRadioId);
                    
                    connection.InfoMessage += Connection_InfoMessage;  // Escuchar mensajes
                    command.ExecuteNonQuery();
                    return RedirectToAction("Index", "Radio");
                }
            }

            catch (Exception ex)
            {
                // Capturar el mensaje de error y mostrarlo en la vista
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
            
        }



        // GET : LISTA
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

       
        // POST: RadioController/Edit
        [HttpPost]
        public IActionResult Editar(Radio model)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Obtener dinámicamente el ID del tipo de equipo 'Radio'
                    var tipoEquipoCommand = new SqlCommand("SELECT ID_tipo_equipo FROM Tipo_Equipos WHERE Tipo_Equipo = 'Radio'", connection);
                    int tipoEquipoRadioId = (int)tipoEquipoCommand.ExecuteScalar(); // Asumimos que siempre hay un registro

                    var command = new SqlCommand("P_ActualizarRadio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Parametros de equipo y detalles equipo
                    command.Parameters.AddWithValue("@ID_equipo", model.ID_equipo);
                    command.Parameters.AddWithValue("@NumeroSerie", model.NumeroSerie);
                    command.Parameters.AddWithValue("@Descripcion", model.Descripcion);
                    command.Parameters.AddWithValue("@id_tipo_equipo", tipoEquipoRadioId); // ID dinámico para Radio
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

                    // Parametros de radio
                    command.Parameters.AddWithValue("ID_Radio", model.ID_Radio);
                    command.Parameters.AddWithValue("Frecuencia", model.Frecuencia);
                    command.Parameters.AddWithValue("Frecuencia_Rango", model.Frecuencia_Rango);
                    command.Parameters.AddWithValue("Modo", model.Modo);
                    command.Parameters.AddWithValue("Ssid", model.Ssid);
                    command.Parameters.AddWithValue("Modulacion", model.Modulacion);
                    command.Parameters.AddWithValue("Potencia", model.Potencia);
                    command.Parameters.AddWithValue("Tx_Power", model.Tx_Power);
                    command.Parameters.AddWithValue("Rx_Level", model.Rx_Level);
                    command.Parameters.AddWithValue("Tx_Freq", model.Tx_Freq);

                    command.ExecuteNonQuery();
                    return RedirectToAction("Index", new { id = model.ID_equipo });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }



        // GET: RadioController/Editar
        [HttpGet]
        public IActionResult Editar(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var model = new Radio();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("P_ObtenerRadio", connection)
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

                        // Radio
                        model.ID_Radio = Convert.ToInt32(reader["ID_Radio"]);
                        model.Frecuencia = reader["Frecuencia"]?.ToString();
                        model.Frecuencia_Rango = reader["Frecuencia_Rango"]?.ToString();
                        model.Modo = reader["Modo"]?.ToString();
                        model.Ssid = reader["Ssid"]?.ToString();
                        model.Modulacion = reader["Modulacion"]?.ToString();
                        model.Potencia = reader["Potencia"]?.ToString();
                        model.Tx_Power = reader["Tx_Power"]?.ToString();
                        model.Rx_Level = reader["Rx_Level"]?.ToString();
                        model.Tx_Freq = reader["Tx_Freq"]?.ToString();
                    }
                }
                model.Proveedores = ObtenerSelectList(connection, "P_ObtenerProveedores", "ID_proveedor", "Nombre");
                model.Modelos = ObtenerSelectList(connection, "P_ObtenerModelos", "ID_modelo", "Nombre_Modelo");
                model.TiposEquipos = ObtenerSelectList(connection, "P_ObtenerTiposEquipos", "ID_tipo_equipo", "Tipo_Equipo");
                model.Areas = ObtenerSelectList(connection, "P_ObtenerAreas", "ID_area", "Nombre_Area");
                model.IPs = ObtenerSelectList(connection, "P_ObtenerIPs", "ID_ip", "IPV4");
            }
            return View(model);
        }



        

        // GET: RadioController/Index
        [HttpGet]
        public IActionResult Index()
        {
            var radioList = new List<Radio>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("P_ObtenerSoloRadios", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var model = new Radio
                            {
                                
                                ID_equipo = reader["ID_equipo"] != DBNull.Value ? (int)reader["ID_equipo"] : 0,
                                NumeroSerie = reader["NumeroSerie"]?.ToString(),
                                Descripcion = reader["Descripcion"]?.ToString(),
                                Estado = reader["Estado"]?.ToString(),
                                Frecuencia = reader["Frecuencia"]?.ToString(),
                                Modulacion = reader["Modulacion"]?.ToString(),
                                Potencia = reader["Potencia"]?.ToString(),
                                Tx_Power = reader["Tx_Power"]?.ToString(),
                                Rx_Level = reader["Rx_Level"]?.ToString()
                            };

                            Console.WriteLine("Número de Serie: " + model.NumeroSerie);
                            radioList.Add(model);
                        }
                    }
                }
            }

            return View(radioList); // Pasar la lista a la vista
        }



        private static void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            // Mostrar cada mensaje enviado por SQL Server
            foreach (SqlError info in e.Errors)
            {
                Console.WriteLine("Mensaje de SQL Server: " + info.Message);
            }
        }
    }
}
        
