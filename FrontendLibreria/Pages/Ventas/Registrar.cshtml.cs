using FrontendLibreria.Adapters.Venta;
using FrontendLibreria.DTOs.VentaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Ventas
{
    public class RegistrarModel : PageModel
    {
        //private readonly ClienteServicio _clienteServicio;
        //private readonly ProductoServicio _productoServicio;
        private readonly IVentaAdapter _ventaAdapter;

        public RegistrarModel(
            //ClienteServicio clienteServicio,
            //ProductoServicio productoServicio,
            IVentaAdapter ventaAdapter)
        {
            //_clienteServicio = clienteServicio;
            //_productoServicio = productoServicio;
            _ventaAdapter = ventaAdapter;
        }

        public void OnGet()
        {
        }

        // --- HU-03 Role A: Real-time search by CI ---
        //public JsonResult OnGetBuscarCliente(string ci)
        //{
        //    if (string.IsNullOrWhiteSpace(ci))
        //        return new JsonResult(new { success = false, message = "CI no proporcionado" });

        //    DataTable clientesSimilares = _clienteServicio.GetAllSimilarId(ci);
        //    var cliente = _clienteServicio.BuscarPorCi(ci);

        //    if (cliente != null)
        //    {
        //        return new JsonResult(new
        //        {
        //            success = true,
        //            cliente = new
        //            {
        //                cliente.Id,
        //                cliente.Nombre,
        //                cliente.ApellidoPaterno,
        //                cliente.ApellidoMaterno
        //            }
        //        });
        //    }

        //    return new JsonResult(new { success = false, message = "Cliente no encontrado" });
        //}

        //[ValidateAntiForgeryToken]
        //public JsonResult OnPostCrearCliente([FromBody] Cliente cliente)
        //{
        //    if (cliente == null)
        //    {
        //        return new JsonResult(new { success = false, message = "Datos inválidos" });
        //    }

        //    cliente.Estado = true;
        //    cliente.FechaRegistro = DateTime.Now;
        //    cliente.IdUsuario = 1;

        //    var result = _clienteServicio.Insert(cliente);

        //    if (result.IsFailure)
        //    {
        //        string fullErrorMessage = "";

        //        foreach (var error in result.Errors)
        //        {
        //            var parts = error.Split(':', 2);

        //            if (parts.Length == 2)
        //            {
        //                var field = parts[0].Trim();
        //                var message = parts[1].Trim();
        //                fullErrorMessage += $"Error in {field}: {message} \n";
        //            }
        //            else
        //            {
        //                fullErrorMessage += $"Error: {error} \n";
        //            }
        //        }

        //        return new JsonResult(new
        //        {
        //            success = false,
        //            message = fullErrorMessage
        //        });
        //    }

        //    var nuevo = _clienteServicio.BuscarPorCi(cliente.Ci);

        //    return new JsonResult(new
        //    {
        //        success = true,
        //        cliente = new
        //        {
        //            nuevo.Id,
        //            nuevo.Nombre,
        //            nuevo.ApellidoPaterno,
        //            nuevo.ApellidoMaterno,
        //            nuevo.Ci
        //        }
        //    });
        //}

        //public JsonResult OnGetBuscarClientesParcial(string ci)
        //{
        //    if (string.IsNullOrWhiteSpace(ci))
        //    {
        //        return new JsonResult(new { success = false, clientes = new List<object>() });
        //    }

        //    var tabla = _clienteServicio.GetAllSimilarId(ci);
        //    var lista = new List<object>();

        //    foreach (DataRow row in tabla.Rows)
        //    {
        //        lista.Add(new
        //        {
        //            id = Convert.ToInt32(row["Id"]),
        //            nombre = row["Nombre"].ToString(),
        //            apellidoPaterno = row["ApellidoPaterno"].ToString(),
        //            apellidoMaterno = row["ApellidoMaterno"] == DBNull.Value ? null : row["ApellidoMaterno"].ToString(),
        //            ci = row["Ci"].ToString()
        //        });
        //    }

        //    return new JsonResult(new
        //    {
        //        success = true,
        //        clientes = lista
        //    });
        //}

        public async Task<JsonResult> OnGetBuscarNombreAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return new JsonResult(new List<object>());

            List<PresentacionProductoVentaDTO> productos =
                await _ventaAdapter.ObtenerPresentacionesPorFraseAsync(termino);

            var listaNombres = productos.Select(producto => new
            {
                texto = producto.Descripcion,
                idProducto = producto.IdProducto,
                idPresentacion = producto.IdPresentacion
            }).ToList();

            return new JsonResult(listaNombres);
        }

        public async Task<IActionResult> OnGetObtenerDetalleProductoAsync(
            string frase,
            int idProducto,
            int idPresentacion)
        {
            if (string.IsNullOrEmpty(frase))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El nombre esta vacio."
                });
            }

            PresentacionProductoVentaDTO? producto =
                await _ventaAdapter.ObtenerPresentacionProductoByIdsAsync(idProducto, idPresentacion);

            if (producto == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el producto."
                });
            }

            return new JsonResult(new
            {
                success = true,
                producto = new
                {
                    idProducto = producto.IdProducto,
                    idPresentacion = producto.IdPresentacion,
                    nombre = !string.IsNullOrWhiteSpace(producto.Nombre)
                        ? producto.Nombre
                        : producto.Descripcion,
                    precioUnitario = producto.PrecioUnitario > 0
                        ? producto.PrecioUnitario
                        : producto.Precio
                }
            });
        }

        public async Task<IActionResult> OnGetImprimirComprobanteAsync(int idVenta)
        {
            if (idVenta <= 0)
            {
                return BadRequest("ID de venta inválido.");
            }

            try
            {
                byte[] pdf = await _ventaAdapter.GenerarComprobantePdfAsync(idVenta);

                if (pdf == null || pdf.Length == 0)
                    return Content("Error: no se pudo generar el comprobante.");

                string nombreArchivo = $"Comprobante_Venta_{idVenta}.pdf";

                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = nombreArchivo,
                    Inline = true
                };

                Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        public class RegistrarVentaDto
        {
            public int IdCliente { get; set; }
            public List<DetalleVentaDTO> Detalles { get; set; } = new List<DetalleVentaDTO>();
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostRegistrarVentaAsync([FromBody] RegistrarVentaDto dto)
        {
            if (dto == null || dto.Detalles == null || !dto.Detalles.Any())
                return new JsonResult(new { success = false, message = "La venta no tiene productos." });

            if (dto.IdCliente <= 0)
                return new JsonResult(new { success = false, message = "Cliente no válido." });

            string? usuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioClaim, out int idUsuario))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se pudo identificar al usuario actual."
                });
            }

            decimal total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            var request = new RegistrarVentaRequestDTO
            {
                Venta = new VentaDTO
                {
                    CiCliente = dto.IdCliente,
                    IdUsuario = idUsuario,
                    Fecha = DateTime.Now,
                    Total = total,
                    Estado = 1
                },
                Detalles = dto.Detalles
            };

            var result = await _ventaAdapter.RegistrarVentaAsync(request);

            if (result != null && result.IsSuccess)
            {
                return new JsonResult(new
                {
                    success = true,
                    idVenta = result.Value,
                    message = "Venta registrada correctamente."
                });
            }

            string mensajeError = result?.Error
                ?? result?.Errors.FirstOrDefault()
                ?? "Error al registrar.";

            return new JsonResult(new
            {
                success = false,
                message = mensajeError
            });
        }
    }
}