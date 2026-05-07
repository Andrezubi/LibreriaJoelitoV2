using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using System.Data;

namespace Servicio_Clientes.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioServicio _usuarioServicio;
        private readonly BitacoraRepositorio _bitacoraRepo;

        public UsuariosController(UsuarioServicio usuarioServicio, BitacoraRepositorio bitacoraRepo)
        {
            _usuarioServicio = usuarioServicio;
            _bitacoraRepo = bitacoraRepo;
        }

        private int GetIdUsuarioFromHeader()
        {
            if (Request.Headers.TryGetValue("X-IdUsuario", out var idStr))
            {
                if (int.TryParse(idStr, out int id)) return id;
            }
            return 0;
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
                    Id = row["Id"]?.ToString(),
                    Nombre = row["Nombre"]?.ToString(),
                    ApellidoPaterno = row["ApellidoPaterno"]?.ToString(),
                    ApellidoMaterno = row["ApellidoMaterno"] == DBNull.Value ? null : row["ApellidoMaterno"]?.ToString(),
                    Ci = row["Ci"]?.ToString(),
                    Complemento = row["Complemento"] == DBNull.Value ? null : row["Complemento"]?.ToString(),
                    FechaNacimiento = row["FechaNacimiento"]?.ToString(),
                    Email = row["Email"]?.ToString(),
                    DireccionDomicilio = row["DireccionDomicilio"] == DBNull.Value ? null : row["DireccionDomicilio"]?.ToString(),
                    Rol = row["Rol"]?.ToString(),
                    Telefono = row["Telefono"] == DBNull.Value ? null : row["Telefono"]?.ToString(),
                    FechaIngreso = row["FechaIngreso"]?.ToString(),
                    NombreUsuario = row["Username"]?.ToString()
                });
            }

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var row = _usuarioServicio.ObtenerPorId(id);
            if (row == null) return NotFound();

            var usuario = new
            {
                Id = Convert.ToInt32(row["Id"]),
                Nombre = row["Nombre"]?.ToString(),
                ApellidoPaterno = row["ApellidoPaterno"]?.ToString(),
                ApellidoMaterno = row["ApellidoMaterno"] == DBNull.Value ? null : row["ApellidoMaterno"]?.ToString(),
                Ci = row["Ci"]?.ToString(),
                Complemento = row["Complemento"] == DBNull.Value ? null : row["Complemento"]?.ToString(),
                FechaNacimiento = row["FechaNacimiento"]?.ToString(),
                Email = row["Email"]?.ToString(),
                DireccionDomicilio = row["DireccionDomicilio"] == DBNull.Value ? null : row["DireccionDomicilio"]?.ToString(),
                Rol = row["Rol"]?.ToString(),
                Telefono = row["Telefono"] == DBNull.Value ? null : row["Telefono"]?.ToString(),
                FechaIngreso = row["FechaIngreso"]?.ToString(),
                NombreUsuario = row["Username"]?.ToString()
            };

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] Usuario usuario)
        {
            var resultado = await _usuarioServicio.InsertarAsync(usuario);

            if (resultado.EsExito)
            {
                // AUDITORÍA
                _bitacoraRepo.Registrar(GetIdUsuarioFromHeader(), "INSERT", "Usuario", $"Nuevo usuario registrado: {usuario.NombreUsuario}");
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
                // AUDITORÍA
                _bitacoraRepo.Registrar(GetIdUsuarioFromHeader(), "UPDATE", "Usuario", $"Usuario actualizado ID: {id}");
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
                // AUDITORÍA
                _bitacoraRepo.Registrar(GetIdUsuarioFromHeader(), "DELETE", "Usuario", $"Usuario eliminado (baja lógica) ID: {id}");
                return Ok(new { mensaje = "Usuario eliminado exitosamente (baja lógica)." });
            }

            return BadRequest(new { mensaje = "No se pudo eliminar el usuario." });
        }

        [HttpGet("estado")]
        public ActionResult<bool> EstadoUsuario([FromQuery] string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return BadRequest(false);
            bool eliminado = _usuarioServicio.EstadoUsuario(nombreUsuario);
            return Ok(eliminado);
        }
    }
}
