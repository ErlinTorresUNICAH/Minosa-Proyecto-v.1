using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class AreasController : Controller
{
    private readonly IConfiguration _configuration;

    public AreasController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Método para obtener las zonas y pasarlas a las vistas
    private List<SelectListItem> ObtenerZonas()
    {
        List<SelectListItem> zonas = new List<SelectListItem>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("SELECT ID_zona, Nombre_Zona FROM Zonas", connection);
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

    // Acción para obtener todas las áreas
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
                    Nombre_Zona = reader["Nombre_Zona"].ToString(),  // Mostrar el nombre de la zona
                    Activo = (bool)reader["Activo"]
                });
            }
        }

        return View(areas);
    }

    // Acción Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Zonas = ObtenerZonas(); // Pasar las zonas a la vista
        return View();
    }

    // Acción Crear (POST)
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

    // Acción Editar (GET)
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

        ViewBag.Zonas = ObtenerZonas();  // Pasar las zonas a la vista
        return View(area);
    }

    // Acción Editar (POST)
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

    // Acción Eliminar (GET)
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

    // Acción Eliminar (POST)
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
        catch (SqlException ex)
        {
            if (ex.Number == 50000)  // Captura el error lanzado por RAISERROR
            {
                ViewBag.Error = ex.Message;
                return View(area);
            }
        }

        return RedirectToAction("Index");
    }
}
