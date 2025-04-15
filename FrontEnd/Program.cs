using Microsoft.EntityFrameworkCore; // Importar Entity Framework Core
using FrontEnd.Models; // Aseg�rate de que este namespace contiene ProyectoVeterinariaContext
using FrontEnd.Services;
using FrontEnd.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configurar el DbContext con la cadena de conexi�n desde appsettings.json
// *** MODIFICADO: Se usa ProyectoVeterinariaContext y el nombre de la conexi�n "PF_G8" ***
builder.Services.AddDbContext<ProyectoVeterinariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PF_G8"))); // Cambiado de "DefaultConnection" a "PF_G8"

// Add services to the container.
builder.Services.AddControllersWithViews();

// Habilitar el uso de sesiones
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar sesiones en memoria

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n de la sesi�n
    // options.Cookie.HttpOnly = true; // Descomentar si se necesita m�s seguridad en cookies
    // options.Cookie.IsEssential = true; // Descomentar si se necesita para GDPR compliance
});

// Registrar el servicio de correo y su interfaz
builder.Services.AddTransient<ICorreoService, CorreoService>();

// Registrar otros servicios necesarios si hay alg�n controlador o servicio adicional
// Si tienes controladores espec�ficos que se necesitan agregar expl�citamente puedes agregarlos aqu�.
// builder.Services.AddTransient<CorreoController, CorreoController>(); // Si tienes un controlador, puedes registrarlo si es necesario

// Se puede agregar el MVC si se necesita, pero ya lo tienes con `AddControllersWithViews`
//builder.Services.AddMvc().AddControllersAsServices(); // Opcional seg�n necesidades

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es de 30 d�as. Puedes cambiar esto para escenarios de producci�n
    // app.UseHsts(); // Descomentar si se usa HSTS (HTTP Strict Transport Security)
}

// Redirecci�n a HTTPS y Static Files suelen ir antes de Routing
// app.UseHttpsRedirection(); // Descomentar si se fuerza HTTPS
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Middleware para usar sesiones (Debe ir despu�s de UseRouting y antes de UseAuthorization/UseEndpoints)

app.UseAuthorization(); // Debe ir despu�s de UseRouting y UseAuthentication/UseSession

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Ruta predeterminada

app.Run();