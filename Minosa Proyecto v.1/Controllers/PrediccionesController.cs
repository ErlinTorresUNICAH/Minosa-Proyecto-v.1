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

            
            try
            {
                
                string scriptPath = _configuration["PythonSettings:ScriptPath"];
               
                string formattedScriptPath = $"\"{scriptPath}\"";


                // Configurar el proceso
                ProcessStartInfo psi = new ProcessStartInfo
                {

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
                    
                    

                   
                    var predicciones = JsonConvert.DeserializeObject<List<Predicciones>>(output);
                    
                    if (predicciones != null)
                    {
                        foreach (var item in predicciones)
                        {
                            //Console.WriteLine(item); 
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
