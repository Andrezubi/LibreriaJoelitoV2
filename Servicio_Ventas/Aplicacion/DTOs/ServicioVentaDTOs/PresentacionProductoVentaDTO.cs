namespace Servicio_Ventas.Aplicacion.DTOs.ServicioVentaDTOs
{
    public class PresentacionProductoVentaDTO
    {
        public int IdProducto { get; set; }
        public int IdPresentacion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
    }
}
