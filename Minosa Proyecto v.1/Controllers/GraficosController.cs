using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Minosa_Proyecto_v._1.Controllers
{

    public class GraficosController : Controller { 

        private readonly IConfiguration _configuration;

        public GraficosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        public IActionResult Index()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            var data = new List<dynamic>();
            using (var connection = new SqlConnection (connectionString))
            {
                var command = new SqlCommand("P_GetEquiposPorTipo_G", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            TipoEquipo = reader["Tipo_Equipo"].ToString(),
                            Cantidad = Convert.ToInt32(reader["Cantidad"])
                        });
                    }
                }
            }
            return View(data);
            /*return Json(data);*/
            /*ViewData["DataJson"] = System.Text.Json.JsonSerializer.Serialize(data); // Convertimos a JSON
            return View();*/
        }

    }
}
