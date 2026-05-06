using FrontendLibreria.Adapters.Cliente;
using FrontendLibreria.Adapters.Venta;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IAdaptadorCliente, AdaptadorCliente>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServicioVentasUrl"]!);
});

//Inyectar el servicio de HttpClient para consumir la API
builder.Services.AddHttpClient<IVentaAdapter, VentaAdapter>(client =>
{
    string? baseUrl = builder.Configuration["ApiSettings:ServicioVentasUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new Exception("No se configuró ApiSettings:ServicioVentasUrl.");

    client.BaseAddress = new Uri(baseUrl);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
