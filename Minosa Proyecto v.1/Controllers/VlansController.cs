using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

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

    // Acción Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    // Acción Crear (POST)
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

    // Acción Editar (GET)
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

    // Acción Editar (POST)
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

    // Acción Eliminar (GET)
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

    // Acción Eliminar (POST)
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
        catch (SqlException ex)
        {
            if (ex.Number == 50000)  // Captura el error lanzado por RAISERROR
            {
                ViewBag.Error = ex.Message;
                return View(vlan);
            }
        }

        return RedirectToAction("Index");
    }


    /*
    //----------------------------------------------------------------------------------------------
    // craecion de las ips 

    public List<DireccionIp> CalculadoraVlans(string ipAddress, int subnetMask, int idVlan)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        uint ipInt = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
        uint mask = ~(uint.MaxValue >> subnetMask);

        uint networkAddress = ipInt & mask;
        uint broadcastAddress = networkAddress | ~mask;
        int numHosts = (int)(~mask - 1);

        List<DireccionIp> ipRange = new List<DireccionIp>();

        for (uint currentIP = networkAddress + 1; currentIP < broadcastAddress; currentIP++)
        {
            IPAddress ipToInsert = new IPAddress(BitConverter.GetBytes(currentIP).Reverse().ToArray());
            ipRange.Add(new DireccionIp
            {
                IPV4 = ipToInsert.ToString(),
                Estado = "Disponible",
                id_vlan = idVlan,
                Activa = true,
                ping = true
            });
        }

        return ipRange;
    }


    // Acción Crear (POST)
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
            command.Parameters.AddWithValue("@ping", direccion.ping);
            command.Parameters.AddWithValue("@Activa", direccion.Activa);
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CrearIps(string ip, int subnetMask, int idVlan)
    {
        var ips = CalculadoraVlans(ip, subnetMask, idVlan);

        string connectionString = _configuration.GetConnectionString("DefaultConnection");

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

    // --------------------------------------------------------------------------------------------------------
    // Método para crear una nueva VLAN y agregar sus direcciones IP
    [HttpPost]
    public IActionResult CrearVlanConIps(string nombreVlan, string subNet, string gateway, string dhcpIni, string dhcpFin, string observaciones, string ip, int subnetMask)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        int idVlan = 0;

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Crear la VLAN y obtener el ID generado
            var command = new SqlCommand("P_GRUD_CrearVlanID", connection); // Asume que existe un procedimiento almacenado para crear VLAN
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Nombre_Vlan", nombreVlan);
            command.Parameters.AddWithValue("@SubNet", subNet);
            command.Parameters.AddWithValue("@Gateway", gateway);
            command.Parameters.AddWithValue("@DhcpIni", dhcpIni);
            command.Parameters.AddWithValue("@DhcpFin", dhcpFin);
            command.Parameters.AddWithValue("@Observaciones", observaciones);

            // Parámetro de salida para obtener el ID generado
            var idVlanParameter = new SqlParameter("@ID_vlan", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(idVlanParameter);

            command.ExecuteNonQuery();
            idVlan = (int)idVlanParameter.Value;
        }

        // Generar y guardar las direcciones IP asociadas a la VLAN
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
                command.Parameters.AddWithValue("@Activa", direccion.Activa);
                command.Parameters.AddWithValue("@ping", direccion.ping);
                command.ExecuteNonQuery();
            }
        }

        return RedirectToAction("Index");
    }
    */


    // Crear vlans con ip *
    public List<DireccionIp> CalculadoraVlans(string ipAddress, int subnetMask, int idVlan)
    {
        // Parse the IP address and convert it to an unsigned integer.
        IPAddress ip = IPAddress.Parse(ipAddress);
        uint ipInt = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);

        // Calculate the subnet mask as an unsigned integer.
        uint mask = ~(uint.MaxValue >> subnetMask);

        // Calculate the network and broadcast addresses.
        uint networkAddress = ipInt & mask;
        uint broadcastAddress = networkAddress | ~mask;

        // Initialize a list to store the generated IP addresses.
        List<DireccionIp> ipRange = new List<DireccionIp>();

        // Loop through the range to create IP addresses, excluding the network and broadcast addresses.
        for (uint currentIP = networkAddress + 1; currentIP < broadcastAddress; currentIP++)
        {
            // Convert the current IP integer to an IPAddress object.
            IPAddress ipToInsert = new IPAddress(BitConverter.GetBytes(currentIP).Reverse().ToArray());

            // Add the IP address to the list with default properties.
            ipRange.Add(new DireccionIp
            {
                IPV4 = ipToInsert.ToString(),
                Estado = "Disponible",
                id_vlan = idVlan,
                Activa = false,
                ping = true
            });
        }

        // Return the list of IP addresses.
        return ipRange;
    }


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


    [HttpPost]
    public IActionResult CrearVlanConIps(string nombreVlan, string subNet, string gateway, string dhcpIni, string dhcpFin, string observaciones, string ip, int subnetMask)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        int idVlan = 0;

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var command = new SqlCommand("P_GRUD_CrearVlanID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Nombre_Vlan", nombreVlan);
            command.Parameters.AddWithValue("@SubNet", subNet);
            command.Parameters.AddWithValue("@Gateway", gateway);
            command.Parameters.AddWithValue("@DhcpIni", dhcpIni);
            command.Parameters.AddWithValue("@DhcpFin", dhcpFin);
            command.Parameters.AddWithValue("@Observaciones", observaciones);

            var idVlanParameter = new SqlParameter("@ID_vlan", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(idVlanParameter);

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

