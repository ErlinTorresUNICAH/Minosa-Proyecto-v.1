using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class CorreoSettingsController : Controller
{
    private readonly IOptionsMonitor<CorreoSettings> _correoSettings;

    public CorreoSettingsController(IOptionsMonitor<CorreoSettings> correoSettings)
    {
        _correoSettings = correoSettings;
    }

    // Muestra la configuración actual
    //public IActionResult Index()
    //{
    //    return View(_correoSettings.CurrentValue);
    //}
    public IActionResult Index()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        var json = System.IO.File.ReadAllText(filePath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        var settings = new CorreoSettings
        {
            SmtpServer = jsonObj["CorreoSettings"]["SmtpServer"],
            SmtpPort = Convert.ToInt32(jsonObj["CorreoSettings"]["SmtpPort"]),
            EmailFrom = jsonObj["CorreoSettings"]["EmailFrom"],
            EmailPassword = jsonObj["CorreoSettings"]["EmailPassword"]
        };

        return View(settings);
    }

    // Muestra el formulario para editar
    //[HttpGet]
    //public IActionResult Editar()
    //{
    //    return View(_correoSettings.CurrentValue);
    //}
    [HttpGet]
    public IActionResult Editar()
    {
        // Leer el archivo de configuración
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        var json = System.IO.File.ReadAllText(filePath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        // Crear el modelo a partir de los valores leídos
        var settings = new CorreoSettings
        {
            SmtpServer = jsonObj["CorreoSettings"]["SmtpServer"],
            SmtpPort = jsonObj["CorreoSettings"]["SmtpPort"] != null ? Convert.ToInt32(jsonObj["CorreoSettings"]["SmtpPort"]) : 0,
            EmailFrom = jsonObj["CorreoSettings"]["EmailFrom"],
            EmailPassword = jsonObj["CorreoSettings"]["EmailPassword"]
        };

        return View(settings);
    }


    // Guarda los cambios en la configuración
    [HttpPost]
    public IActionResult Editar(CorreoSettings settings)
    {
        if (ModelState.IsValid)
        {
            // Aquí actualizas el archivo appsettings.json o lo que uses
            UpdateAppSettings(settings);
            return RedirectToAction("Index");
        }
        return View(settings);
    }

    private void UpdateAppSettings(CorreoSettings settings)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        var json = System.IO.File.ReadAllText(filePath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        // Verifica que "CorreoSettings" no sea nulo
        if (jsonObj["CorreoSettings"] == null)
        {
            jsonObj["CorreoSettings"] = new Newtonsoft.Json.Linq.JObject();
        }

        jsonObj["CorreoSettings"]["SmtpServer"] = settings.SmtpServer;
        jsonObj["CorreoSettings"]["SmtpPort"] = settings.SmtpPort.ToString();
        jsonObj["CorreoSettings"]["EmailFrom"] = settings.EmailFrom;
        jsonObj["CorreoSettings"]["EmailPassword"] = settings.EmailPassword;

        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        System.IO.File.WriteAllText(filePath, output);
    }

}
