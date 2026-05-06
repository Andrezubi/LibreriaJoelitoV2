using Servicio_Ventas.Dominio.Modelos;

namespace Servicio_Ventas.Aplicacion.DTOs.ServicioVentaDTOs
{
    public class RegistrarVentaRequestDto
    {
        public Venta Venta { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
