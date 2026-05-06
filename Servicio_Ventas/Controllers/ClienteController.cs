using Microsoft.AspNetCore.Mvc;
using Servicio_Ventas.Aplicacion.Servicios;
using Servicio_Ventas.Dominio.Modelos;

namespace Servicio_Ventas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteServicio _clienteServicio;

        public ClienteController(ClienteServicio clienteServicio)
        {
            _clienteServicio = clienteServicio;
        }

        [HttpGet]
        public IActionResult ObtenerTodo() => Ok(_clienteServicio.ObtenerTodo());

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var cliente = _clienteServicio.ObtenerPorId(id);
            if (cliente.Id == 0) return NotFound();
            return Ok(cliente);
        }

        [HttpGet("buscar-ci/{ci}")]
        public IActionResult ObtenerPorCi(string ci)
        {
            var cliente = _clienteServicio.ObtenerPorCi(ci);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpGet("similares-ci/{ci}")]
        public IActionResult ObtenerSimilarCi(string ci)
            => Ok(_clienteServicio.ObtenerSimilarCi(ci));

        [HttpPost]
        public IActionResult Insertar([FromBody] Cliente cliente)
        {
            var result = _clienteServicio.Insertar(cliente);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok(new { success = true, id = result.Value });
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] Cliente cliente)
        {
            cliente.Id = id;
            var result = _clienteServicio.Actualizar(cliente);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id, [FromBody] Cliente cliente)
        {
            cliente.Id = id;
            _clienteServicio.Eliminar(cliente);
            return Ok();
        }
    }
}