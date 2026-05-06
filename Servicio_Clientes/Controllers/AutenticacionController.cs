using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AutenticacionController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;
        private readonly ILogger<AutenticacionController> _logger;

        public AutenticacionController(UsuarioServicio usuarioServicio, ILogger<AutenticacionController> logger)
        {
            _usuarioServicio = usuarioServicio;
            _logger = logger;
        }

        [HttpPost("login")]
        public ActionResult<LoginResultado> Login([FromBody] LoginPeticion request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NombreUsuario) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new LoginResultado { Exito = false, Mensaje = "Usuario y contraseña son requeridos." });

            var result = _usuarioServicio.Login(request.NombreUsuario, request.Contrasena);

            if (!result.Exito)
            {
                _logger.LogInformation("Login fallido para el usuario {Usuario}: {Mensaje}", request.NombreUsuario, result.Mensaje);
                return Unauthorized(result);
            }

            // Opcional: setear cookie HttpOnly para que JwtBearer la lea
            Response.Cookies.Append("AuthToken", result.Token ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(result);
        }
    }
}
