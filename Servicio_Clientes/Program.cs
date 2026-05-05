using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Infraestructura.FactoryCreators;
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

// Registro de IServicioEmail
builder.Services.AddTransient<IServicioEmail, ServicioEmail>();

// Registro de IServicioPdf
builder.Services.AddScoped<IServicioPdf, ServicioPdf>();

// Registro de IHasherContrasena
builder.Services.AddTransient<IHasherContrasena, HasherSimple>();

// Inyección del repositorio concreto via Factory Method (mismo patrón que Servicio_Ventas)
builder.Services.AddScoped<UsuarioRepository>(provider => {
    return new UsuarioCreadorRepositorio().CrearRepositorio();
});

// Add services to the container.
builder.Services.AddControllers();

// Swagger para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para permitir llamadas desde el Frontend (Razor Pages)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Inyección del Servicio Token
builder.Services.AddScoped<IServicioToken, ServicioToken>();

// Inyección del Servicio de Usuario (fachada)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

        // Leer el token desde la cookie HttpOnly
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
bd.Initiate(connectionString!);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
