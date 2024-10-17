using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

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
}
