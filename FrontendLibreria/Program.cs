using FrontendLibreria.Adapters.Servicio2Adapters;
using FrontendLibreria.Adapters.Venta;
using Microsoft.AspNetCore.Authentication.Cookies;
using FrontendLibreria.Adapters.Producto;
using FrontendLibreria.Adapters.Cliente;
using FrontendLibreria.Adapters.Venta;
using FrontendLibreria.Adapters.Marca;
using FrontendLibreria.Adapters.Servicio2Adapters;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

// ── Autenticación con Cookie HttpOnly ──────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Servicio2/InicioSesion";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Servicio2/InicioSesion";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddHttpClient<IAdaptadorCliente, AdaptadorCliente>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

builder.Services.AddAuthorization();
builder.Services.AddHttpClient<IUsuarioServicioAdapter, UsuarioServicioAdapter>(client => {
    client.BaseAddress = new Uri(builder.Configuration["UsuarioService:BaseUrl"] ?? "https://localhost:7002");
});

builder.Services.AddHttpClient<IAdaptadorCliente, AdaptadorCliente>(client => {
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});
builder.Services.AddHttpClient<IVentaAdapter, VentaAdapter>(client =>
{
    {
        string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");
        client.BaseAddress = new Uri(baseUrl);
    }
});

builder.Services.AddHttpClient<IAdaptadorProducto, AdaptadorProducto>(client => {
    string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl)) throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<IAdaptadorMarca, AdaptadorMarca>(client => {
    string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl)) throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");
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

app.UseAuthentication();
app.UseAuthorization();

// Filtro de cambio de contraseña
app.Use(async (context, next) => {
    var path = context.Request.Path.Value ?? "";
    var user = context.User;
    bool estaAutenticado = user.Identity?.IsAuthenticated == true;
    bool debeCambiar = user.FindFirst("MustChangePassword")?.Value == "True";

    bool esRutaPermitida = path.StartsWith("/Usuarios/CambiarContrasena", StringComparison.OrdinalIgnoreCase) ||
                           path.StartsWith("/Usuarios/Logout", StringComparison.OrdinalIgnoreCase) ||
                           path.StartsWith("/Servicio2/InicioSesion", StringComparison.OrdinalIgnoreCase) ||
                           path.Contains("/css") || path.Contains("/js") || path.Contains("/lib");

    if (estaAutenticado && debeCambiar && !esRutaPermitida)
    {
        context.Response.Redirect("/Usuarios/CambiarContrasena");
        return;
    }
    await next();
});

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();