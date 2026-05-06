using Microsoft.AspNetCore.Mvc;
using Servicio_Ventas.Aplicacion.Servicios;
using Servicio_Ventas.Dominio.Modelos;

namespace Servicio_Ventas.Controllers
{
    // Controllers/ProductosController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoServicio _productoServicio;
        private readonly PresentacionServicio _presentacionServicio;

        public ProductoController(ProductoServicio productoServicio, PresentacionServicio presentacionServicio)
        {
            _productoServicio = productoServicio;
            _presentacionServicio = presentacionServicio;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_productoServicio.ObtenerTodo());

        [HttpGet("categorias")]
        public IActionResult GetCategorias() { /* query categorias */ }

        [HttpGet("marcas")]
        public IActionResult GetMarcas() { /* query marcas */ }

        [HttpGet("presentaciones")]
        public IActionResult GetPresentaciones() => Ok(_presentacionServicio.ObtenerTodo());

        [HttpPost]
        public IActionResult Create([FromBody] Producto producto) { /* ... */ }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Producto producto)
        {
            var result = _productoServicio.Actualizar(producto);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody] Producto producto)
        {
            _productoServicio.Eliminar(producto);
            return Ok();
        }

        [HttpPost("{id}/presentaciones")]
        public IActionResult AgregarPresentacion(int id, [FromBody] AgregarPresentacionDto dto)
        {
            var result = _productoServicio.AsociarNuevaPresentacion(
                id, dto.IdPresentacion, dto.FactorConversion, dto.PrecioVenta, dto.IdUsuario);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok();
        }
    }
}
