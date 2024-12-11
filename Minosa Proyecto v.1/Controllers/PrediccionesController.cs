using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Minosa_Proyecto_v._1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Minosa_Proyecto_v._1.Controllers
{
    public class PrediccionesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PrediccionesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult PredecirEstado(string ip, string hora)
        {

            Console.WriteLine($"IP: {ip}");
            Console.WriteLine($"Última Hora de Ping: {hora}");
            try
            {
                // Ruta al script Python
                //string scriptPath = @"~/PythonScripts/prediction_service.py";
                //string scriptPath = IWebHostEnvironment("~/PythonScripts/prediction_service.py");
                string scriptPath = _configuration["PythonSettings:ScriptPath"];
                //string scriptPath = @"C:\Users\erlin\source\repos\Minosa Proyecto v.1\Minosa Proyecto v.1\PythonScripts\prediction_service.py";

                // Enciérralo entre comillas
                string formattedScriptPath = $"\"{scriptPath}\"";

                // Configurar el proceso
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    //FileName = @"C:\Users\erlin\AppData\Local\Programs\Python\Python312\python.exe",
                    FileName = _configuration["PythonSettings:FileName"],
                    Arguments = $"{formattedScriptPath} \"{ip}\" \"{hora}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Ejecutar el script
                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();

                    // Leer la salida del script
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    Console.WriteLine("Output: " + output);
                    /* (!string.IsNullOrEmpty(error))
                    {
                        return BadRequest($"Error en el script Python: {error}");
                    }*/

                    // Convertir el JSON a un objeto C#
                    var predicciones = JsonConvert.DeserializeObject<List<Predicciones>>(output);
                    //var predicciones = output;
                    Console.WriteLine("Datos transformados:");
                    if (predicciones != null)
                    {
                        foreach (var item in predicciones)
                        {
                            Console.WriteLine(item); // Esto depende de cómo esté implementado el método ToString en PrediccionResult
                        }
                    }
                    else
                    {
                        Console.WriteLine("La lista está vacía o es nula.");
                    }
                    return View("PrediccionesResultado", predicciones);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al ejecutar el script: {ex.Message}");
            }
        }

        
    }
}
