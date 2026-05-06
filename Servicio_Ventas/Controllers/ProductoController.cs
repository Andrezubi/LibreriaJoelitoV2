using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Servicio_Ventas.Aplicacion.DTOs;
using Servicio_Ventas.Aplicacion.Results;
using Servicio_Ventas.Aplicacion.Servicios;

using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Dominio.Validadores;
using Servicio_Ventas.Infrestructura.Persistencia;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public IActionResult GetCategorias() {

            string query = @"SELECT Id, Nombre FROM categoria WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);

            List<CategoriaDto> result = new List<CategoriaDto>();
            MySqlDataReader reader = RepositorioBD.Instancia.ExecuteReader(cmd);
            while (reader.Read())
            {

                result.Add(
                    new CategoriaDto
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader["Nombre"].ToString(),
                        
                    });
            }
            return Ok(result);


        }

        [HttpGet("marcas")]
        public IActionResult GetMarcas() {
            string query = @"SELECT Id, Nombre FROM marca WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);

            List<Marca> result = new List<Marca>();
            MySqlDataReader reader = RepositorioBD.Instancia.ExecuteReader(cmd);
            while (reader.Read())
            {

                result.Add(
                    new Marca
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader["Nombre"].ToString(),

                    });
            }
            return Ok(result);
        }

        [HttpPost("categorias")]
        public IActionResult InsertCategorias([FromBody] CategoriaDto data) {
            data.Nombre = data.Nombre?.Trim();
            if (string.IsNullOrWhiteSpace(data.Nombre)) return BadRequest(new { errores = "No se puede estar en blanco la categoria" });

            try
            {
                var errors = ExtraValidador.ValidarNombreCategoria(data.Nombre);
                if (errors.Any()) return BadRequest(new { errores = errors });

                string query = "INSERT INTO categoria (Nombre, IdUsuario) VALUES (@nombre, @idUsuario);";
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Parameters.AddWithValue("@nombre", data.Nombre);
                cmd.Parameters.AddWithValue("@idUsuario",data.IdUsuario);
                int res=RepositorioBD.Instancia.ExecuteNonQuery(cmd);
                if (res >= 1) {
                    return Ok(new { success = true });
                }
                return BadRequest(new { errores = "No se ingeso correctamente" });
            }
            catch (Exception ex)
            {
               return BadRequest(ex);
            }
        } 

        [HttpGet("presentaciones")]
        public IActionResult GetPresentaciones() => Ok(_presentacionServicio.ObtenerTodo());



        [HttpPost("{idPresentacion}/{factorConversion}/{precioVenta}")]
        public IActionResult Create(int idPresentacion,int factorConversion,decimal precioVenta ,[FromBody] Producto producto) 
        {
            var result = _productoServicio.Insertar(producto,idPresentacion,factorConversion,precioVenta);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok(new {success=true});
            /* ... */ 
        }

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
        public IActionResult AgregarPresentacion(int id, [FromBody] PresentacionProductoDto dto)
        {
            var result = _productoServicio.AsociarNuevaPresentacion(
                id, dto.IdPresentacion, dto.FactorConversion, dto.Precio, dto.IdUsuario);
            if (result.IsFailure) return BadRequest(new { errores = result.Errors });
            return Ok();
        }
    }
}
