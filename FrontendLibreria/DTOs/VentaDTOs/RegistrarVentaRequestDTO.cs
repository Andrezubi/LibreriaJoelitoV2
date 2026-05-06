namespace FrontendLibreria.DTOs.VentaDTOs
{
    public class RegistrarVentaRequestDTO
    {
        public VentaDTO Venta { get; set; } = new VentaDTO();
        public List<DetalleVentaDTO> Detalles { get; set; } = new List<DetalleVentaDTO>();
    }
}