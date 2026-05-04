using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;

        public AutenticacionController(UsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginPeticion peticion)
        {
            if (string.IsNullOrWhiteSpace(peticion.NombreUsuario) || string.IsNullOrWhiteSpace(peticion.Contrasena))
            {
                return BadRequest(new { exito = false, mensaje = "Credenciales incompletas." });
            }

            var resultado = _usuarioServicio.Login(peticion.NombreUsuario, peticion.Contrasena);

            if (resultado.Exito)
            {
                return Ok(new
                {
                    exito = true,
                    token = resultado.Token,
                    debeCambiarContrasena = resultado.DebeCambiarContrasena,
                    rol = resultado.Rol
                });
            }

            return Unauthorized(new { exito = false, mensaje = resultado.Mensaje });
        }
    }
}
