using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Aplicacion.Servicios;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Dominio.Validadores;
using Servicio_Ventas.Infrestructura.FactoriaCreadores;
using Servicio_Ventas.Infrestructura.Persistencia;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using Servicio_Ventas.Infrestructura.ServiciosExternos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddScoped<ClienteRepositorio>(provider => {
    return new ClienteCreadorRepositorio().CrearRepositorio();
});
builder.Services.AddScoped<BitacoraRepositorio>();



//Inyeccion Servicios
builder.Services.AddScoped<RealizarVentaServicio>();
builder.Services.AddScoped<AnularVentaServicio>();
builder.Services.AddScoped<ConsultaVentaServicio>();
builder.Services.AddScoped<GestionInventarioServicio>();
builder.Services.AddScoped<PresentacionServicio>();
builder.Services.AddScoped<ProductoServicio>();



//Inyeccion Validadores
builder.Services.AddScoped<ProductoValidador>();


builder.Services.AddScoped<ClienteServicio>();
builder.Services.AddScoped<ClienteValidador>(); 
// Add services to the container.

builder.Services.AddControllers();

// JWT Configuracion
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
