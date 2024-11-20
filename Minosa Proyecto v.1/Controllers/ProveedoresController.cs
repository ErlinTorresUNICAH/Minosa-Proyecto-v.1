using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using System.Collections.Generic;
using System.Data;

public class ProveedoresController : Controller
{
    private readonly IConfiguration _configuration;

    public ProveedoresController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Obtener todos los proveedores
    public IActionResult Index()
    {
        List<Proveedor> proveedores = new List<Proveedor>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerProveedores", connection);
            command.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                proveedores.Add(new Proveedor
                {
                    ID_proveedor = (int)reader["ID_proveedor"],
                    Nombre = reader["Nombre"].ToString(),
                    Direccion = reader["Direccion"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    Correo = reader["Correo"].ToString()
                });
            }
        }
        return View(proveedores);
    }

    // CRUD Proveedores

    // Funcion Leer proveedor
    // El Index es la funcion



    // Funcion Crear (GET)
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }
    // Funcion Crear (POST)
    [HttpPost]
    public IActionResult Crear(Proveedor proveedor)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_CrearProveedor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            command.Parameters.AddWithValue("@Direccion", proveedor.Direccion);
            command.Parameters.AddWithValue("@Telefono", proveedor.Telefono);
            command.Parameters.AddWithValue("@Correo", proveedor.Correo);
            connection.Open();
            command.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }




    // Funcion Editar (GET)
    [HttpGet]
    public IActionResult Editar(int id)
    {
        Proveedor proveedor = new Proveedor();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerProveedorPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_proveedor", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                proveedor.ID_proveedor = (int)reader["ID_proveedor"];
                proveedor.Nombre = reader["Nombre"].ToString();
                proveedor.Direccion = reader["Direccion"].ToString();
                proveedor.Telefono = reader["Telefono"].ToString();
                proveedor.Correo = reader["Correo"].ToString();
            }
        }
        return View(proveedor);
    }
    // Funcion Editar (POST)
    [HttpPost]
    public IActionResult Editar(Proveedor proveedor)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ActualizarProveedor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_proveedor", proveedor.ID_proveedor);
            command.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            command.Parameters.AddWithValue("@Direccion", proveedor.Direccion);
            command.Parameters.AddWithValue("@Telefono", proveedor.Telefono);
            command.Parameters.AddWithValue("@Correo", proveedor.Correo);
            connection.Open();
            command.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }



    // Funcion Eliminar (GET)
    [HttpGet]
    public IActionResult Eliminar(int id)
    {
        Proveedor proveedor = new Proveedor();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_ObtenerProveedorPorID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_proveedor", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                proveedor.ID_proveedor = (int)reader["ID_proveedor"];
                proveedor.Nombre = reader["Nombre"].ToString();
                proveedor.Direccion = reader["Direccion"].ToString();
                proveedor.Telefono = reader["Telefono"].ToString();
                proveedor.Correo = reader["Correo"].ToString();
            }
        }
        return View(proveedor);
    }
    // Funcion Eliminar (POST)
    [HttpPost]
    public IActionResult Eliminar(Proveedor proveedor)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            var command = new SqlCommand("P_GRUD_EliminarProveedor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ID_proveedor", proveedor.ID_proveedor);
            connection.Open();
            command.ExecuteNonQuery();
        }
        return RedirectToAction("Index");
    }

}
