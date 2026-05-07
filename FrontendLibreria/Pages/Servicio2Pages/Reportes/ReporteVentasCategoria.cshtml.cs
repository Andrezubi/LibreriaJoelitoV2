using FrontendLibreria.Adapters.Servicio2Adapters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendLibreria.Pages.Servicio2Pages.Reportes
{
    [Authorize]
    public class ReporteVentasCategoriaModel : PageModel
    {
        private readonly IUsuarioServicioAdapter _adapter;

        public ReporteVentasCategoriaModel(IUsuarioServicioAdapter adapter)
        {
            _adapter = adapter;
        }

        public string Error { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(
            DateTime fechaDesde, DateTime fechaHasta, string format)
        {
            if (fechaDesde > fechaHasta)
            {
                Error = "La fecha desde no puede ser mayor que la fecha hasta.";
                return Page();
            }

            var token = User.FindFirst("Token")?.Value ?? string.Empty;

            var archivo = await _adapter.GenerarReporteVentasCategoria(
                fechaDesde, fechaHasta, format, token);

            if (archivo == null || archivo.Length == 0)
            {
                Error = "No se pudo generar el reporte. Verifique que existan ventas en ese rango.";
                return Page();
            }

            if (format == "excel")
                return File(archivo,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ReporteVentas_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}.xlsx");

            return File(archivo, "application/pdf",
                $"ReporteVentas_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}.pdf");
        }
    }
}
