using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infrestructura.FactoriaCreadores;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using Servicio_Ventas.Infrestructura.ServiciosExternos;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddScoped<IPdfServicio, PdfServicio>();
builder.Services.AddScoped<VentaRepository>(provider => {
    return new VentaCreatorRepository().CreateRepository();
});
builder.Services.AddScoped<DetalleVentaRepository>(provider => {
    return new DetalleVentaCreatorRepository().CreateRepository();
});

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
