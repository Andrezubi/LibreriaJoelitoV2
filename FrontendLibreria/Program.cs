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
    string baseUrl = builder.Configuration["UsuarioService:BaseUrl"] ?? "https://localhost:7002";

    // En desarrollo, permitir usar HTTP si está configurado
    if (builder.Environment.IsDevelopment() && baseUrl.Contains("localhost"))
    {
        // Intentar HTTPS primero, pero permite certificados autofirmados
        if (baseUrl.StartsWith("https"))
        {
            baseUrl = baseUrl; // Mantener HTTPS con validación personalizada
        }
    }

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30); // Aumentar timeout para desarrollo
})
.ConfigureHttpMessageHandlerBuilder(httpBuilder =>
{
    // En desarrollo, permitir certificados autofirmados
    if (builder.Environment.IsDevelopment())
    {
        httpBuilder.PrimaryHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
    }
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