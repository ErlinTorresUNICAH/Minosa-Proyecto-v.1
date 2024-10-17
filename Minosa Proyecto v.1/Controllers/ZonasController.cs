using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

public class ZonasController : Controller
{
    private readonly IConfiguration _configuration;

    public ZonasController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Acción para obtener todas las zonas
    public IActionResult Index()
    {
        List<Zona> zonas = new List<Zona>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerZonas", connection);
            command.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                zonas.Add(new Zona
                {
                    ID_zona = (int)reader["ID_zona"],
                    Nombre_Zona = reader["Nombre_Zona"].ToString(),
                    Descripcion_Zona = reader["Descripcion_Zona"].ToString(),
                    Activo = (bool)reader["Activo"],
                    Creacion_Zona = (DateTime)reader["Creacion_Zona"]
                });
            }
        }

        return View(zonas);
    }

    // Acción Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    // Acción Crear (POST)
    [HttpPost]
    public IActionResult Crear(Zona zona)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_CrearZona", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Nombre_Zona", zona.Nombre_Zona);
            command.Parameters.AddWithValue("@Descripcion_Zona", zona.Descripcion_Zona);
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    // Acción Editar (GET)
    [HttpGet]
    public IActionResult Editar(int id)
    {
        Zona zona = new Zona();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerZonaPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_zona", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                zona.ID_zona = (int)reader["ID_zona"];
                zona.Nombre_Zona = reader["Nombre_Zona"].ToString();
                zona.Descripcion_Zona = reader["Descripcion_Zona"].ToString();
                zona.Activo = (bool)reader["Activo"];
            }
        }

        return View(zona);
    }

    // Acción Editar (POST)
    [HttpPost]
    public IActionResult Editar(Zona zona)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ActualizarZona", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_zona", zona.ID_zona);
            command.Parameters.AddWithValue("@Nombre_Zona", zona.Nombre_Zona);
            command.Parameters.AddWithValue("@Descripcion_Zona", zona.Descripcion_Zona);
            command.Parameters.AddWithValue("@Activo", zona.Activo);
            connection.Open();
            command.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    // Acción Eliminar (GET)
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        Zona zona = new Zona();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerZonaPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_zona", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                zona.ID_zona = (int)reader["ID_zona"];
                zona.Nombre_Zona = reader["Nombre_Zona"].ToString();
                zona.Descripcion_Zona = reader["Descripcion_Zona"].ToString();
            }
        }

        return View(zona);
    }

    // Acción Eliminar (POST)
    [HttpPost]
    public IActionResult Eliminar(Zona zona)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_EliminarZona", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_zona", zona.ID_zona);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 50000)  // Captura el error lanzado por RAISERROR
            {
                ViewBag.Error = ex.Message;
                return View(zona);
            }
        }

        return RedirectToAction("Index");
    }
}
