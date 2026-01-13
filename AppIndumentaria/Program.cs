using AppIndumentaria.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Configura el servicio de DbContext para SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// =========================================================================
// AGREGAR SERVICIOS DE IDENTITY
// =========================================================================
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// =========================================================================


// Add services to the container.
// =========================================================================
// MODIFICACIÓN: APLICAR FILTRO DE AUTORIZACIÓN GLOBAL
// =========================================================================
builder.Services.AddControllersWithViews(options =>
{
    // 1. Crear la política que requiere que el usuario esté autenticado.
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // 2. Aplicar la política a TODOS los Controladores y Acciones por defecto.
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages();
// =========================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// =========================================================================
// MIDDLEWARE DE AUTENTICACIN Y AUTORIZACIN
// =========================================================================
app.UseAuthentication();
app.UseAuthorization();
// =========================================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();