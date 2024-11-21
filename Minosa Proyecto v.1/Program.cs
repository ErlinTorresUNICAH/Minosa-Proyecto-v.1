using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


// Añadir servicios al contenedor
builder.Services.AddControllersWithViews();

// Agregar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";  // Página de acceso
        options.AccessDeniedPath = "/Login/AccessDenied"; // Página de acceso denegado (opcional)
    });
builder.Services.AddHostedService<ActividadBackgroundService>();

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
