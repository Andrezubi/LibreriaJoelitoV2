namespace Servicio_Clientes.Dominio.Models.DTOs
{
    public class ReporteVentaCategoriaDto
    {
        public string Categoria { get; set; } = string.Empty;
        public int TotalUnidades { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class FiltroReporteDto
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
