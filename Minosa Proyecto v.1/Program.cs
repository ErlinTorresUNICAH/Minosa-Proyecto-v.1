using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Agregar archivos de configuración
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();



// Añadir servicios al contenedor
builder.Services.AddControllersWithViews();

// Agregar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";  // Página de acceso
        options.AccessDeniedPath = "/Login/AccessDenied"; // Página de acceso denegado (opcional)
    });

//builder.Services.Configure<RoleSettings>(builder.Configuration.GetSection("Roles"));

builder.Services.AddHostedService<ActividadBackgroundService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); // Política para rol Admin
});



builder.Services.Configure<CorreoSettings>(builder.Configuration.GetSection("CorreoSettings"));
var correoSettings = builder.Configuration.GetSection("CorreoSettings").Get<CorreoSettings>();
Console.WriteLine($"SMTP Server: {correoSettings.SmtpServer}");
Console.WriteLine($"SMTP Port: {correoSettings.SmtpPort}");
Console.WriteLine($"Email From: {correoSettings.EmailFrom}");

var app = builder.Build();

// Configuracion HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Habilitar la autenticación
app.UseAuthorization();  // Habilitar la autorización

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
