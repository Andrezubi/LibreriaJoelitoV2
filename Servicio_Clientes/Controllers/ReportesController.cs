using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio_Clientes.Aplicacion.Servicios;

namespace Servicio_Clientes.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly ReporteServicio _reporteServicio;

        public ReportesController(ReporteServicio reporteServicio)
        {
            _reporteServicio = reporteServicio;
        }

        [HttpGet("ventas-categoria")]
        public IActionResult GenerarReporte(
            [FromQuery] DateTime fechaDesde,
            [FromQuery] DateTime fechaHasta,
            [FromQuery] string format = "pdf")
        {
            if (fechaDesde > fechaHasta)
                return BadRequest(new { mensaje = "FechaDesde no puede ser mayor que FechaHasta." });

            var usuario = User.Identity?.Name ?? "sistema";

            if (format.ToLower() == "excel")
            {
                var excel = _reporteServicio.GenerarExcel(fechaDesde, fechaHasta, usuario);
                return File(excel,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ReporteVentas_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}.xlsx");
            }
            else
            {
                var pdf = _reporteServicio.GenerarPdf(fechaDesde, fechaHasta, usuario);
                return File(pdf, "application/pdf",
                    $"ReporteVentas_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}.pdf");
            }
        }
    }
}
