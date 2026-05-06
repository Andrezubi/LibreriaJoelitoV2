using FrontendLibreria.Adapters.Servicio2Adapters;
using FrontendLibreria.Adapters.Venta;
using Microsoft.AspNetCore.Authentication.Cookies;
using FrontendLibreria.Adapters.Producto;
using FrontendLibreria.Adapters.Cliente;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

// ── Autenticación con Cookie HttpOnly ──────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Servicio2/InicioSesion";  // ✅ Coincide con @page en InicioSesion.cshtml
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Servicio2/InicioSesion";  // ✅ Sincronizado
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddHttpClient<IAdaptadorCliente, AdaptadorCliente>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

builder.Services.AddAuthorization();

// ── Adapter hacia Servicio 2 (Clientes/Usuarios) ───────────────────────────
builder.Services.AddHttpClient<IUsuarioServicioAdapter, UsuarioServicioAdapter>(client =>
{
    // URL del Servicio 2 (puerto 7002 HTTPS / 5002 HTTP)
    client.BaseAddress = new Uri(builder.Configuration["UsuarioService:BaseUrl"]
                                 ?? "https://localhost:7002");
});

// ── Adapter hacia Servicio Ventas ──────────────────────────────────────────
builder.Services.AddHttpClient<IVentaAdapter, VentaAdapter>(client =>
{
    string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");

    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<IAdaptadorProducto, AdaptadorProducto>(client =>
{
    string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");

    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // ← debe ir ANTES de Authorization
app.UseAuthorization();

app.MapRazorPages();

app.Run();