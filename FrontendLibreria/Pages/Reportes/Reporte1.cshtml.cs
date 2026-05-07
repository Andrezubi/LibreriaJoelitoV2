using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using FrontendLibreria.DTOs.VentaDTOs;

namespace FrontendLibreria.Pages.Reportes
{
    public class Reporte1Model : PageModel
    {
        private readonly IVentaAdapter _ventaAdapter;
        public Reporte1Model(IVentaAdapter ventaAdapter)
        {
            _ventaAdapter = ventaAdapter;
        }

        public List<Reporte1DTO> ListaServicios { get; set; } = new();
        public async Task OnGetAsync()
        {
            ListaServicios = await _ventaAdapter.ObtenerReporteServiciosAsync();
        }
    }

}