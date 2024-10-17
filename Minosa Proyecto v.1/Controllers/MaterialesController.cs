using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class MaterialesController : Controller
{
    private readonly IConfiguration _configuration;

    public MaterialesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Método para obtener las áreas y pasarlas a las vistas
    private List<SelectListItem> ObtenerAreas()
    {
        List<SelectListItem> areas = new List<SelectListItem>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("SELECT ID_area, Nombre_Area FROM Areas", connection);
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                areas.Add(new SelectListItem
                {
                    Value = reader["ID_area"].ToString(),
                    Text = reader["Nombre_Area"].ToString()
                });
            }
        }

        return areas;
    }

    // Acción para obtener todos los materiales
    public IActionResult Index()
    {
        List<Material> materiales = new List<Material>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerMateriales", connection);
            command.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                materiales.Add(new Material
                {
                    ID_Material = (int)reader["ID_Material"],
                    Descripcion = reader["Descripcion"].ToString(),
                    Cantidad = (int)reader["Cantidad"],
                    Nombre_Area = reader["Nombre_Area"].ToString()
                });
            }
        }

        return View(materiales);
    }

    // Acción Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Areas = ObtenerAreas();  // Pasar las áreas a la vista
        return View();
    }

    // Acción Crear (POST)
    [HttpPost]
    public IActionResult Crear(Material material)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_CrearMaterial", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Descripcion", material.Descripcion);
            command.Parameters.AddWithValue("@Cantidad", material.Cantidad);
            command.Parameters.AddWithValue("@id_area", material.id_area);
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    // Acción Editar (GET)
    [HttpGet]
    public IActionResult Editar(int id)
    {
        Material material = new Material();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerMaterialPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_Material", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                material.ID_Material = (int)reader["ID_Material"];
                material.Descripcion = reader["Descripcion"].ToString();
                material.Cantidad = (int)reader["Cantidad"];
                material.id_area = (int)reader["id_area"];
            }
        }

        ViewBag.Areas = ObtenerAreas();  // Pasar las áreas a la vista
        return View(material);
    }

    // Acción Editar (POST)
    [HttpPost]
    public IActionResult Editar(Material material)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ActualizarMaterial", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_Material", material.ID_Material);
            command.Parameters.AddWithValue("@Descripcion", material.Descripcion);
            command.Parameters.AddWithValue("@Cantidad", material.Cantidad);
            command.Parameters.AddWithValue("@id_area", material.id_area);
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    // Acción Eliminar (GET)
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        Material material = new Material();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerMaterialPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_Material", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                material.ID_Material = (int)reader["ID_Material"];
                material.Descripcion = reader["Descripcion"].ToString();
                material.Cantidad = (int)reader["Cantidad"];
                material.Nombre_Area = reader["Nombre_Area"].ToString();
            }
        }

        return View(material);
    }

    // Acción Eliminar (POST)
    [HttpPost]
    public IActionResult Eliminar(Material material)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_EliminarMaterial", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_Material", material.ID_Material);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000)  // Captura el error lanzado por RAISERROR
            {
                ViewBag.Error = ex.Message;
                return View(material);
            }
        }

        return RedirectToAction("Index");
    }
}
