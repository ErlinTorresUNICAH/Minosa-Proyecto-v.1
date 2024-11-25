using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using System.Collections.Generic;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Obtener la lista de roles
        private List<SelectListItem> ObtenerRoles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT ID_rol, Nombre_Rol FROM Roles", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(new SelectListItem
                    {
                        Value = reader["ID_rol"].ToString(),
                        Text = reader["Nombre_Rol"].ToString()
                    });
                }
            }

            return roles;
        }

        // Listar Usuarios
        public IActionResult Index()
        {
            List<Usuario> usuarios = new List<Usuario>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerUsuarios", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        ID_usuario = (int)reader["ID_usuario"],
                        Nombre_Usuario = reader["Nombre_Usuario"].ToString(),
                        Nombre_Rol = reader["Nombre_Rol"].ToString()
                    });
                }
            }

            return View(usuarios);
        }

        // Crear Usuario (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Roles = ObtenerRoles();
            return View();
        }

        // Crear Usuario (POST)
        [HttpPost]
        public IActionResult Crear(Usuario usuario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_CrearUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Nombre_Usuario", usuario.Nombre_Usuario);
                command.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                command.Parameters.AddWithValue("@id_rol", usuario.id_rol);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Editar Usuario (GET)
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Usuario usuario = new Usuario();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerUsuarioPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_usuario", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    usuario.ID_usuario = (int)reader["ID_usuario"];
                    usuario.Nombre_Usuario = reader["Nombre_Usuario"].ToString();
                    usuario.Contrasena = reader["Contrasena"].ToString();
                    usuario.id_rol = (int)reader["id_rol"];
                }
            }

            ViewBag.Roles = ObtenerRoles();
            return View(usuario);
        }

        // Editar Usuario (POST)
        [HttpPost]
        public IActionResult Editar(Usuario usuario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ActualizarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_usuario", usuario.ID_usuario);
                command.Parameters.AddWithValue("@Nombre_Usuario", usuario.Nombre_Usuario);
                command.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                command.Parameters.AddWithValue("@id_rol", usuario.id_rol);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Eliminar Usuario (GET)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            Usuario usuario = new Usuario();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_ObtenerUsuarioPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_usuario", id);
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    usuario.ID_usuario = (int)reader["ID_usuario"];
                    usuario.Nombre_Usuario = reader["Nombre_Usuario"].ToString();
                    usuario.Nombre_Rol = reader["id_rol"].ToString();
                }
            }

            return View(usuario);
        }

        // Eliminar Usuario (POST)
        [HttpPost]
        public IActionResult Eliminar(Usuario usuario)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("P_GRUD_EliminarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID_usuario", usuario.ID_usuario);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
