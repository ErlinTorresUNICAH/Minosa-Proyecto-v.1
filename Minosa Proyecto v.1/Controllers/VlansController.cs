using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using Minosa_Proyecto_v._1.Models;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize]
    public class VlansController : Controller
    {
        private readonly IConfiguration _configuration;

        public VlansController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Acción para obtener todas las VLANs
        public IActionResult Index()
        {
            List<Vlan> vlans = new List<Vlan>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerVlans", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    vlans.Add(new Vlan
                    {
                        ID_vlan = (int)reader["ID_vlan"],
                        Nombre_Vlan = reader["Nombre_Vlan"].ToString(),
                        SubNet = reader["SubNet"].ToString(),
                        Gateway = reader["Gateway"].ToString(),
                        DhcpIni = reader["DhcpIni"].ToString(),
                        DhcpFin = reader["DhcpFin"].ToString(),
                        Observaciones = reader["Observaciones"].ToString(),
                        Activo = (bool)reader["Activo"],
                        Creacion_vlan = (DateTime)reader["Creacion_vlan"]
                    });
                }
            }

            return View(vlans);
        }


        //CRUD Vlans

        // Funcion Leer
        // Esta funcion es el Index



        // Funcion Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        // Funcion Crear (POST)
        [HttpPost]
        public IActionResult Crear(Vlan vlan)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearVlan", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Vlan", vlan.Nombre_Vlan);
                command.Parameters.AddWithValue("@SubNet", vlan.SubNet);
                command.Parameters.AddWithValue("@Gateway", vlan.Gateway);
                command.Parameters.AddWithValue("@DhcpIni", vlan.DhcpIni);
                command.Parameters.AddWithValue("@DhcpFin", vlan.DhcpFin);
                command.Parameters.AddWithValue("@Observaciones", vlan.Observaciones);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }



        // Funcion Editar (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Vlan vlan = new Vlan();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerVlanPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_vlan", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    vlan.ID_vlan = (int)reader["ID_vlan"];
                    vlan.Nombre_Vlan = reader["Nombre_Vlan"].ToString();
                    vlan.SubNet = reader["SubNet"].ToString();
                    vlan.Gateway = reader["Gateway"].ToString();
                    vlan.DhcpIni = reader["DhcpIni"].ToString();
                    vlan.DhcpFin = reader["DhcpFin"].ToString();
                    vlan.Observaciones = reader["Observaciones"].ToString();
                    vlan.Activo = (bool)reader["Activo"];
                }
            }

            return View(vlan);
        }
        // Funcion Editar (POST)
        [HttpPost]
        public IActionResult Editar(Vlan vlan)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarVlan", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_vlan", vlan.ID_vlan);
                command.Parameters.AddWithValue("@Nombre_Vlan", vlan.Nombre_Vlan);
                command.Parameters.AddWithValue("@SubNet", vlan.SubNet);
                command.Parameters.AddWithValue("@Gateway", vlan.Gateway);
                command.Parameters.AddWithValue("@DhcpIni", vlan.DhcpIni);
                command.Parameters.AddWithValue("@DhcpFin", vlan.DhcpFin);
                command.Parameters.AddWithValue("@Observaciones", vlan.Observaciones);
                command.Parameters.AddWithValue("@Activo", vlan.Activo);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }




        // Funcion Eliminar (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Vlan vlan = new Vlan();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerVlanPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_vlan", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    vlan.ID_vlan = (int)reader["ID_vlan"];
                    vlan.Nombre_Vlan = reader["Nombre_Vlan"].ToString();
                }
            }

            return View(vlan);
        }
        // Funcion Eliminar (POST)
        [HttpPost]
        public IActionResult Eliminar(Vlan vlan)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand("P_GRUD_EliminarVlan", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_vlan", vlan.ID_vlan);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            // Captura el error lanzado por si se quiere eliminar una vlan con llave asignada
            catch (SqlException ex)
            {
                if (ex.Number >= 50000)  
                {
                    ViewBag.Error = ex.Message;
                    return View(vlan);
                }
                throw;
            }

            return RedirectToAction("Index");
        }



        // Calculadora de Vlans para generar direcciones IP
        public List<DireccionIp> CalculadoraVlans(string ipAddress, int subnetMask, int idVlan)
        {
            // Analizar la dirección IP y convertirla a un entero sin signo
            IPAddress ip = IPAddress.Parse(ipAddress);
            uint ipInt = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);

            // Calcula la máscara de subred como un entero sin signo
            uint mask = ~(uint.MaxValue >> subnetMask);

            // Calcular las direcciones de red y de difusión
            uint networkAddress = ipInt & mask;
            uint broadcastAddress = networkAddress | ~mask;

            // Inicializar una lista para almacenar las direcciones IP generadas
            List<DireccionIp> ipRange = new List<DireccionIp>();

            // Hacer un bucle a través del rango para crear direcciones IP, excluyendo las direcciones de red y de difusión
            for (uint currentIP = networkAddress + 1; currentIP < broadcastAddress; currentIP++)
            {
                // Convierte el número entero IP actual a un objeto IPAddress
                IPAddress ipToInsert = new IPAddress(BitConverter.GetBytes(currentIP).Reverse().ToArray());

                // Añadir la dirección IP a la lista con las propiedades por defecto
                ipRange.Add(new DireccionIp
                {
                    IPV4 = ipToInsert.ToString(),
                    Estado = "Disponible",
                    id_vlan = idVlan,
                    Activa = false,
                    ping = true
                });
            }

            //Devuelve la lista de direcciones IP
            return ipRange;
        }

        // Función para calcular los campos de la VLAN
        [HttpPost]
        public JsonResult CalculateVlanFields(string ip, int subnetMask)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            uint ipInt = BitConverter.ToUInt32(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);
            uint mask = ~(uint.MaxValue >> subnetMask);

            uint networkAddress = ipInt & mask;
            uint broadcastAddress = networkAddress | ~mask;

            string subNet = new IPAddress(BitConverter.GetBytes(networkAddress).Reverse().ToArray()).ToString();
            string gateway = new IPAddress(BitConverter.GetBytes(networkAddress + 1).Reverse().ToArray()).ToString();
            string dhcpIni = new IPAddress(BitConverter.GetBytes(networkAddress + 2).Reverse().ToArray()).ToString();
            string dhcpFin = new IPAddress(BitConverter.GetBytes(broadcastAddress - 1).Reverse().ToArray()).ToString();

            return Json(new { subNet, gateway, dhcpIni, dhcpFin });
        }

        // Función para crear una VLAN con direcciones IP
        [HttpPost]
        public IActionResult CrearVlanConIps(string Nombre_Vlan, string subNet, string gateway, string dhcpIni, string dhcpFin, string observaciones, string ip, int subnetMask)
        {
            Console.WriteLine("Datos", Nombre_Vlan, subNet, gateway, dhcpIni, dhcpFin, observaciones, ip, subnetMask);
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            int idVlan = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var command = new SqlCommand("P_GRUD_CrearVlanID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Vlan", Nombre_Vlan);
                command.Parameters.AddWithValue("@SubNet", subNet);
                command.Parameters.AddWithValue("@Gateway", gateway);
                command.Parameters.AddWithValue("@DhcpIni", dhcpIni);
                command.Parameters.AddWithValue("@DhcpFin", dhcpFin);
                command.Parameters.AddWithValue("@Observaciones", observaciones);

                var idVlanParameter = new SqlParameter("@ID_vlan", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(idVlanParameter);
                Console.WriteLine("Datos", Nombre_Vlan,  subNet,  gateway,  dhcpIni,  dhcpFin,  observaciones,  ip,  subnetMask);
                command.ExecuteNonQuery();
                idVlan = (int)idVlanParameter.Value;
            }

            var ips = CalculadoraVlans(ip, subnetMask, idVlan);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var direccion in ips)
                {
                    var command = new SqlCommand("P_GRUD_CrearDireccionIp", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IPV4", direccion.IPV4);
                    command.Parameters.AddWithValue("@Estado", direccion.Estado);
                    command.Parameters.AddWithValue("@id_vlan", direccion.id_vlan);
                    command.Parameters.AddWithValue("@ping", direccion.ping);
                    command.Parameters.AddWithValue("@Activa", direccion.Activa);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


    }

}