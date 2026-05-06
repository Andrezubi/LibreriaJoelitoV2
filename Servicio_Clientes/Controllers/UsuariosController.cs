using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;

        public UsuariosController(UsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet]
        public IActionResult ObtenerTodo()
        {
            var dt = _usuarioServicio.ObtenerTodo();
            var usuarios = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                usuarios.Add(new
                {
                    Id = row["Id"],
                    Nombre = row["Nombre"],
                    ApellidoPaterno = row["ApellidoPaterno"],
                    ApellidoMaterno = row["ApellidoMaterno"],
                    Ci = row["Ci"],
                    Complemento = row["Complemento"],
                    FechaNacimiento = row["FechaNacimiento"],
                    Email = row["Email"],
                    DireccionDomicilio = row["DireccionDomicilio"],
                    Rol = row["Rol"],
                    Telefono = row["Telefono"],
                    FechaIngreso = row["FechaIngreso"]
                });
            }

            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] Usuario usuario)
        {
            var resultado = await _usuarioServicio.InsertarAsync(usuario);

            if (resultado.EsExito)
            {
                return Ok(new { mensaje = "Usuario registrado exitosamente." });
            }

            return BadRequest(new { errores = resultado.Errores });
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Usuario usuario)
        {
            usuario.Id = id;
            var resultado = _usuarioServicio.Actualizar(usuario);

            if (resultado.EsExito)
            {
                return Ok(new { mensaje = "Usuario actualizado exitosamente." });
            }

            return BadRequest(new { errores = resultado.Errores });
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            // Usamos un idUsuario dummy o el del usuario autenticado si estuviera disponible.
            // Lo ideal sería extraerlo del token JWT, pero por ahora lo pasamos como 1.
            var usuario = new Usuario { Id = id, IdUsuario = 1 }; 
            var filasAfectadas = _usuarioServicio.Eliminar(usuario);

            if (filasAfectadas > 0)
            {
                return Ok(new { mensaje = "Usuario eliminado exitosamente (baja lógica)." });
            }

            return BadRequest(new { mensaje = "No se pudo eliminar el usuario." });
        }
    }
}
