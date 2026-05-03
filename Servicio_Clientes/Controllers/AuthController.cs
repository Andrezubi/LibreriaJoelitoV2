using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;

        public AuthController(UsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Credenciales incompletas." });
            }

            var result = _usuarioServicio.Login(request.Username, request.Password);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    token = result.Token,
                    mustChangePassword = result.MustChangePassword,
                    rol = result.Rol
                });
            }

            return Unauthorized(new { success = false, message = result.Message });
        }
    }
}
