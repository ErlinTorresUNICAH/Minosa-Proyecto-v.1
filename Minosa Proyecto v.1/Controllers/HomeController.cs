using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minosa_Proyecto_v._1.Models;
using System.Diagnostics;

namespace Minosa_Proyecto_v._1.Controllers
{
    // Home Controller se utukuza para poder redirigir a la vista principal de la aplicacion
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
