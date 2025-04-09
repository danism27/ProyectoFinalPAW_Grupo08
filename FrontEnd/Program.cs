using Microsoft.EntityFrameworkCore; // Importar Entity Framework Core
using FrontEnd.Models; // Asegúrate que este namespace contiene ProyectoVeterinariaContext
using FrontEnd.Services;
using FrontEnd.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configurar el DbContext con la cadena de conexión desde appsettings.json
// *** MODIFICADO: Se usa ProyectoVeterinariaContext y el nombre de la conexión "PF_G8" ***
builder.Services.AddDbContext<ProyectoVeterinariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PF_G8"))); // Cambiado de "DefaultConnection" a "PF_G8"

// Add services to the container.
builder.Services.AddControllersWithViews();

// Habilitar el uso de sesiones
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar sesiones en memoria

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    // options.Cookie.HttpOnly = true; // Descomentar si se necesita más seguridad en cookies
    // options.Cookie.IsEssential = true; // Descomentar si se necesita para GDPR compliance
});

// No es necesario volver a agregar esto si ya se hizo arriba
// builder.Services.AddControllersWithViews(); 

// Registrar el servicio de correo y su interfaz
builder.Services.AddTransient<ICorreoService, CorreoService>();

// Registrar el controlador de correo (aunque esto generalmente no es necesario si usa el descubrimiento estándar de controladores)
// builder.Services.AddTransient<CorreoController, CorreoController>(); // Puedes comentar o eliminar esta línea si no es estrictamente necesaria

// testing - Esta línea generalmente no es necesaria en .NET 6+ a menos que tengas una configuración muy específica
//builder.Services.AddMvc().AddControllersAsServices(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts(); // Descomentar si se usa HSTS
}

// HTTPS Redirection y Static Files suelen ir antes de Routing
// app.UseHttpsRedirection(); // Descomentar si se fuerza HTTPS
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Middleware para usar sesiones (Debe ir después de UseRouting y antes de UseAuthorization/UseEndpoints)

app.UseAuthorization(); // Debe ir después de UseRouting y UseAuthentication/UseSession

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();