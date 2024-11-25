﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minosa_Proyecto_v._1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Minosa_Proyecto_v._1.Controllers
{
    //Controler de login
public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Nombre_Usuario, string Contrasena)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "Connection string is not configured properly.";
                return View();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("P_ObtenerUsuarioRol", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Nombre_Usuario", Nombre_Usuario));
                    command.Parameters.Add(new SqlParameter("@Contrasena", Contrasena));

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, Nombre_Usuario),
                            new Claim(ClaimTypes.NameIdentifier, reader["ID_usuario"].ToString()),
                            new Claim(ClaimTypes.Role, reader["Nombre_Rol"].ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Crear las propiedades de autenticación
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // Mantener la sesión iniciada
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Configurar el tiempo de expiración
                        };

                        // Iniciar la sesión con la cookie de autenticación
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                      new ClaimsPrincipal(claimsIdentity),
                                                      authProperties);

                        // Usuario autenticado, redirigir a la página principal
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al conectar con la base de datos: " + ex.Message;
                return View();
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
