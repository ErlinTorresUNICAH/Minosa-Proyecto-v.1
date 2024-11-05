using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class DireccionesIpController : Controller
{
    private readonly IConfiguration _configuration;

    public DireccionesIpController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Método para obtener las VLANs y pasarlas a las vistas
    private List<SelectListItem> ObtenerVlans()
    {
        List<SelectListItem> vlans = new List<SelectListItem>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("SELECT ID_vlan, Nombre_Vlan FROM vlans", connection);
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

    // Acción Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Vlans = ObtenerVlans();  // Pasar las VLANs a la vista
        return View();
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
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    // Acción Editar (GET)
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

        ViewBag.Vlans = ObtenerVlans();  // Pasar las VLANs a la vista
        return View(direccion);
    }

    // Acción Editar (POST)
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

    // Acción Eliminar (GET)
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

    // Acción Eliminar (POST)
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
        catch (SqlException ex)
        {
            if (ex.Number == 50000)  // Captura el error lanzado por RAISERROR
            {
                ViewBag.Error = ex.Message;
                return View(direccion);
            }
        }

        return RedirectToAction("Index");
    }
}
