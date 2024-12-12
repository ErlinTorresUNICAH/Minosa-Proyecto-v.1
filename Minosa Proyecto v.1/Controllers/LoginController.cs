using Microsoft.AspNetCore.Mvc;
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
            int attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

            // Verificar si hay un tiempo de bloqueo establecido
            var lockoutEndString = HttpContext.Session.GetString("LockoutEnd");
            if (!string.IsNullOrEmpty(lockoutEndString) && DateTime.TryParse(lockoutEndString, out var lockoutEnd))
            {
                if (lockoutEnd > DateTime.Now)
                {
                    var minutesRemaining = (lockoutEnd - DateTime.Now).TotalMinutes;
                    ViewBag.Error = $"Demasiados intentos fallidos. Intenta de nuevo en {minutesRemaining:F2} minutos.";
                    return View();
                }
                else
                {
                    // Si el tiempo de bloqueo ha expirado, reiniciar los valores
                    HttpContext.Session.Remove("LockoutEnd");
                    HttpContext.Session.SetInt32("LoginAttempts", 0);
                    attempts = 0;
                }
            }

            if (attempts >= 10)
            {
                HttpContext.Session.SetString("LockoutEnd", DateTime.Now.AddMinutes(30).ToString("o"));
                ViewBag.Error = "Demasiados intentos fallidos. Intenta de nuevo en un momento.";
                return View();
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.Error = "Connection string is not configurado correctamente.";
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
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                      new ClaimsPrincipal(claimsIdentity),
                                                      authProperties);

                        // Reiniciar los valores de sesión al iniciar sesión correctamente
                        HttpContext.Session.SetInt32("LoginAttempts", 0);
                        HttpContext.Session.Remove("LockoutEnd");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        attempts++;
                        HttpContext.Session.SetInt32("LoginAttempts", attempts);

                        if (attempts >= 10)
                        {
                            HttpContext.Session.SetString("LockoutEnd", DateTime.Now.AddMinutes(30).ToString("o"));
                            ViewBag.Error = "Demasiados intentos fallidos. Intenta de nuevo en unos minuto.";
                        }
                        else
                        {
                            ViewBag.Error = "Usuario o contraseña incorrectos.";
                        }

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



        [Authorize]
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
