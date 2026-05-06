using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Encriptacion;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly IServicioToken _servicioToken; 
        private readonly ILogger<AuthController> _logger;
        private readonly UsuarioServicio _usuarioServicio;

        public AuthController(UsuarioRepository usuarioRepo, IServicioToken servicioToken, ILogger<AuthController> logger, UsuarioServicio usuarioServicio)
        {
            _usuarioRepo = usuarioRepo;
            _servicioToken = servicioToken;
            _logger = logger;
            _usuarioServicio = usuarioServicio;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginPeticion request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.NombreUsuario) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new LoginResultado
                {
                    Exito = false,
                    Mensaje = "Usuario y contraseña son requeridos."
                });

            // ← Ahora sí usa BCrypt internamente
            var resultado = _usuarioServicio.Login(request.NombreUsuario, request.Contrasena);

            if (!resultado.Exito)
            {
                _logger.LogInformation("Login fallido para {Usuario}: {Mensaje}",
                    request.NombreUsuario, resultado.Mensaje);
                return Unauthorized(resultado);
            }

            // Setear cookie HttpOnly
            Response.Cookies.Append("AuthToken", resultado.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            _logger.LogInformation("Login exitoso para {Usuario}", request.NombreUsuario);
            return Ok(resultado);
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

