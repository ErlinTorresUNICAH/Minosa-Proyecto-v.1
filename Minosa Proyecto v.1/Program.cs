using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Agregar archivos de configuraci�n
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();



// A�adir servicios al contenedor
builder.Services.AddControllersWithViews();

// Agregar soporte para la sesi�n
builder.Services.AddDistributedMemoryCache(); // Necesario para usar la sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Duraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // Hacer la cookie solo accesible por HTTP
    options.Cookie.IsEssential = true; // Necesario para que la cookie est� disponible
});



// Agregar autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";  // P�gina de acceso
        options.AccessDeniedPath = "/Login/AccessDenied"; // P�gina de acceso denegado (opcional)
    });

//builder.Services.Configure<RoleSettings>(builder.Configuration.GetSection("Roles"));

builder.Services.AddHostedService<ActividadBackgroundService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); // Pol�tica para rol Admin
});



builder.Services.Configure<CorreoSettings>(builder.Configuration.GetSection("CorreoSettings"));
var correoSettings = builder.Configuration.GetSection("CorreoSettings").Get<CorreoSettings>();


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


app.UseAuthentication(); // Habilitar la autenticaci�n
app.UseAuthorization();  // Habilitar la autorizaci�n
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
