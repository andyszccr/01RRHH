using Microsoft.EntityFrameworkCore;
using RRHH.Models;
using RRHH.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbrrhhContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// Agregar configuración de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de expirar la sesión
    options.Cookie.HttpOnly = true; // La cookie no será accesible desde JavaScript
    options.Cookie.IsEssential = true; // La cookie es esencial para el funcionamiento de la aplicación
});

// Registrar el filtro de autorización
builder.Services.AddScoped<AutorizacionFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar middleware de sesiones
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CuentaUsuario}/{action=Login}/{id?}");

app.Run();
