namespace FrontendLibreria.DTOs.VentaDTOs
{
    public class VentaDTO
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public int IdUsuario { get; set; }
    }
}