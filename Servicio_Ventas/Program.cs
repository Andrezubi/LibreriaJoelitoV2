
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Servicios;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Dominio.Validadores;
using Servicio_Ventas.Infrestructura.FactoriaCreadores;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using Servicio_Ventas.Infrestructura.ServiciosExternos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddScoped<IPdfServicio, PdfServicio>();
builder.Services.AddScoped<VentaRepositorio>(provider => {
    return new VentaCreadorRepositorio().CrearRepositorio();
});
builder.Services.AddScoped<DetalleVentaRepositorio>(provider => {
    return new DetalleVentaCreadorRepositorio().CrearRepositorio();
});
builder.Services.AddScoped<ProductoRepositorio>(provider => {
    return new ProductoCreadorRepositorio().CrearRepositorio();
});
builder.Services.AddScoped<PresentacionRepositorio>(provider => {
    return new PresentacionCreadorRepositorio().CrearRepositorio();
});
builder.Services.AddScoped<PresentacionProductoRepositorio>(provider => {
    return new PresentacionProductoCreadorRepositorio().CrearRepositorio();
});


//Inyeccion Servicios
builder.Services.AddScoped<RealizarVentaServicio>();
builder.Services.AddScoped<AnularVentaServicio>();
builder.Services.AddScoped<ConsultaVentaServicio>();
builder.Services.AddScoped<GestionInventarioServicio>();
builder.Services.AddScoped<PresentacionServicio>();
builder.Services.AddScoped<ProductoServicio>();



//Inyeccion Validadores
builder.Services.AddScoped<ProductoValidador>();


// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

//CONFIG BD
var bd = RepositorioBD.Instancia;

// select connection string from appsettings
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
bd.Initiate(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Servicio Ventas API V1");
        c.RoutePrefix = "swagger"; // optional but explicit
    });
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
