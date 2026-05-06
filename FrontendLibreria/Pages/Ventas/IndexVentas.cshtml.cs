using FrontendLibreria.Adapters.Venta;
using FrontendLibreria.DTOs.VentaDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Ventas
{
    //[Authorize(Roles = "Administrador,Empleado")]
    public class IndexVentasModel : PageModel
    {
        private readonly IVentaAdapter _ventaAdapter;

        public List<VentaDTO> Ventas { get; set; } = new List<VentaDTO>();

        [TempData]
        public string? MensajeExito { get; set; }

        public IndexVentasModel(IVentaAdapter ventaAdapter)
        {
            _ventaAdapter = ventaAdapter;
        }

        public async Task OnGetAsync()
        {
            Ventas = await _ventaAdapter.CargarVentasAsync();
        }

        public async Task<IActionResult> OnGetExportarPdfAsync(int idVenta)
        {
            if (idVenta <= 0)
                return BadRequest("ID de venta inválido.");

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

        public async Task<IActionResult> OnPostAnularAsync(int idVenta)
        {
            if (idVenta <= 0)
                return RedirectToPage();

            //string? empleadoClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (!int.TryParse(empleadoClaim, out int idEmpleado))
            //{
            //    MensajeExito = "No se pudo identificar al empleado actual.";
            //    return RedirectToPage();
            //}
            var idEmpleado = 1; // Reemplaza con el ID del empleado actual

            var resultado = await _ventaAdapter.AnularVentaAsync(idVenta, idEmpleado);

            if (resultado != null && resultado.IsSuccess)
            {
                MensajeExito = $"La venta #{idVenta} ha sido anulada y el stock fue restaurado correctamente.";
            }
            else
            {
                string mensajeError = resultado?.Error
                    ?? resultado?.Errors.FirstOrDefault()
                    ?? "Error al anular la venta.";

                MensajeExito = $"Hubo un problema al anular: {mensajeError}";
            }

            return RedirectToPage();
        }

        public async Task<JsonResult> OnGetObtenerDetalleVentaAsync(int idVenta)
        {
            try
            {
                VentaCompletaDTO? resultado = await _ventaAdapter.ObtenerVentaCompletaAsync(idVenta);

                if (resultado == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No se encontró la venta."
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    venta = new
                    {
                        idVenta = resultado.Venta.Id,
                        ciCliente = resultado.Venta.CiCliente,
                        nombreCliente = resultado.Venta.NombreCliente,
                        fecha = resultado.Venta.Fecha.ToString("dd/MM/yyyy"),
                        empleado = resultado.Venta.NombreEmpleado,
                        total = resultado.Venta.Total
                    },
                    detalles = resultado.Detalles.Select(detalle => new
                    {
                        producto = detalle.Producto,
                        presentacion = detalle.Presentacion,
                        cantidad = detalle.Cantidad,
                        precioUnitario = detalle.PrecioUnitario,
                        subtotal = detalle.Subtotal
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}