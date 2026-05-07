using FrontendLibreria.Adapters.Cliente;
using FrontendLibreria.Adapters.Marca;
using FrontendLibreria.Adapters.Producto;
using FrontendLibreria.Adapters.Servicio2Adapters;
using FrontendLibreria.Adapters.Venta;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Servicio2Pages/InicioSesion");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Servicio2Pages/InicioSesion";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Servicio2Pages/InicioSesion";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IUsuarioServicioAdapter, UsuarioServicioAdapter>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UsuarioService:BaseUrl"] ?? "https://localhost:7002");
});

builder.Services.AddHttpClient<IAdaptadorCliente, AdaptadorCliente>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

builder.Services.AddHttpClient<IVentaAdapter, VentaAdapter>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

builder.Services.AddHttpClient<IAdaptadorProducto, AdaptadorProducto>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

builder.Services.AddHttpClient<IAdaptadorMarca, AdaptadorMarca>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
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

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var user = context.User;
    bool estaAutenticado = user.Identity?.IsAuthenticated == true;
    bool debeCambiar = user.FindFirst("MustChangePassword")?.Value == "True";

    bool esRutaPermitida =
        path.StartsWith("/Usuarios/CambiarContrasena", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/Usuarios/Logout", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/Servicio2Pages/InicioSesion", StringComparison.OrdinalIgnoreCase) ||
        path.Contains("/css") ||
        path.Contains("/js") ||
        path.Contains("/lib");

    if (estaAutenticado && debeCambiar && !esRutaPermitida)
    {
        context.Response.Redirect("/Usuarios/CambiarContrasena");
        return;
    }

    await next();
});

app.MapRazorPages();

app.Run();