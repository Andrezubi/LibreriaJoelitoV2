using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace FrontendLibreria.Pages.Reporte1
{
    public class Reporte1Model : PageModel
    {
        // Esta es la lista que usará la vista para renderizar la tabla
        public List<ReporteServiceViewModel> ListaServicios { get; set; } = new();

        public void OnGet()
        {
            // DATOS ESTÁTICOS PARA PRUEBAS (Hardcoded)
            // Aquí es donde luego llamarás a tu servicio/repositorio
            ListaServicios = new List<ReporteServiceViewModel>
            {
                new ReporteServiceViewModel {
                    Nro = 1,
                    NombreServicio = "Manicure",
                    CostoBs = 80.00m,
                    Descripcion = "Manos",
                    Categoria = "Servicios de Belleza"
                },
                new ReporteServiceViewModel {
                    Nro = 2,
                    NombreServicio = "Pedicure",
                    CostoBs = 85.00m,
                    Descripcion = "Pies",
                    Categoria = "Servicios de Belleza"
                }
            };
        }
    }

    public class ReporteServiceViewModel
    {
        public int Nro { get; set; }
        public string NombreServicio { get; set; }
        public decimal CostoBs { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
    }
}