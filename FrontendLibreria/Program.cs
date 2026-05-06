using FrontendLibreria.Adapters.Servicio2Adapters;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthorization();

// ── Adapter hacia Servicio 2 ───────────────────────────────────────────────
builder.Services.AddHttpClient<IUsuarioServicioAdapter, UsuarioServicioAdapter>(client =>
{
    // URL del Servicio 2 (puerto 7002 HTTPS / 5002 HTTP)
    client.BaseAddress = new Uri(builder.Configuration["UsuarioService:BaseUrl"]
                                 ?? "https://localhost:7002");
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

// Filtro global: bloquea navegación si MustChangePassword = true
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var user = context.User;

    bool estaAutenticado = user.Identity?.IsAuthenticated == true;
    bool debeCambiar = user.FindFirst("MustChangePassword")?.Value == "True";

    // Rutas permitidas aunque deba cambiar contraseña
    bool esRutaPermitida =
        path.StartsWith("/Usuarios/CambiarContrasena", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/Usuarios/Logout", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/Servicio2/InicioSesion", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/img", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase);

    if (estaAutenticado && debeCambiar && !esRutaPermitida)
    {
        context.Response.Redirect("/Usuarios/CambiarContrasena");
        return;
    }

    await next();
});


app.MapRazorPages();

app.Run();