using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Aplicacion.Validators;
using Servicio_Clientes.Dominio.Interfaces;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.FactoryCreators;
using Servicio_Clientes.Infraestructura.Persistencia;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using Servicio_Clientes.Infraestructura.Encryptacion;
using Servicio_Clientes.Infraestructura.ServiciosExternos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Servicio_Clientes.Infraestructura.Persistencia.BD;

var builder = WebApplication.CreateBuilder(args);

// Licencia para Pdfs
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Registro de IEmailService
builder.Services.AddTransient<IEmailService, EmailService>();

// Registro de IPdfService
builder.Services.AddScoped<IPdfService, PdfService>();

// Registro de Password Hasher
builder.Services.AddTransient<IPasswordHasher, SimpleHasher>();

// Dependency inyection IRepository Usuarios
builder.Services.AddScoped<IRepository<Usuario>>(provider => {
    return new UsuarioCreatorRepository().CreateRepository();
});
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();

// Add services to the container.
builder.Services.AddControllers();

// Dependency inyection Token service
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<UsuarioServicio>();

// AGREGAR AUTENTICACION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };

        // 🔥 CLAVE: leer el token desde la cookie
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

// Inicializar conexión a base de datos
var bd = ConexionBD.Instancia;
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
bd.Initiate(connectionString);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
