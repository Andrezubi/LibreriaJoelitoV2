using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Dominio.Models;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly IServicioToken _servicioToken; 
        private readonly ILogger<AuthController> _logger;

        public AuthController(UsuarioRepository usuarioRepo, IServicioToken servicioToken, ILogger<AuthController> logger)
        {
            _usuarioRepo = usuarioRepo;
            _servicioToken = servicioToken;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResultado>> Login([FromBody] LoginPeticion request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NombreUsuario) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new LoginResultado { Exito = false, Mensaje = "Usuario y contraseña son requeridos." });
            var usuario = await Task.Run(() => _usuarioRepo.ObtenerDatosLogin(request.NombreUsuario));
            if (usuario == null)
            {
                _logger.LogInformation("Login fallido: usuario no encontrado {Usuario}", request.NombreUsuario);
                return Unauthorized(new LoginResultado { Exito = false, Mensaje = "Credenciales incorrectas." });
            }

            // Comparación de contraseña (temporal en texto plano)
            bool passwordOk = !string.IsNullOrEmpty(usuario.Contrasena) && request.Contrasena == usuario.Contrasena;
            // Recomendado: passwordOk = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);

            if (!passwordOk)
            {
                _logger.LogInformation("Login fallido: contraseña incorrecta {Usuario}", request.NombreUsuario);
                return Unauthorized(new LoginResultado { Exito = false, Mensaje = "Credenciales incorrectas." });
            }

            // Generar token con IServicioToken o devolver token de prueba
            var token = _servicioToken != null ? _servicioToken.GenerarToken(Convert.ToString(usuario.Id), usuario.NombreUsuario, usuario.Rol) : "token-de-prueba";

            var result = new LoginResultado
            {
                Exito = true,
                Token = token,
                Rol = usuario.Rol ?? string.Empty,
                DebeCambiarContrasena = usuario.DebeCambiarContrasena,
                Mensaje = "Login exitoso"
            };

            // Opcional: setear cookie HttpOnly para que JwtBearer la lea
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(result);
        }

        [HttpGet("estado")]
        public async Task<ActionResult<bool>> EstadoUsuario([FromQuery] string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return BadRequest(false);
            var usuario = await Task.Run(() => _usuarioRepo.ObtenerDatosLogin(nombreUsuario));
            // Si usuario es null -> no existe o está inactivo -> considerarlo eliminado = true
            return Ok(usuario == null ? true : false);
        }
    }

}

