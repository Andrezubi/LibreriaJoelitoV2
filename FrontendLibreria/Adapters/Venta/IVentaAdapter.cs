using FrontendLibreria.DTOs.VentaDTOs;
using FrontendLibreria.Adapters;

namespace FrontendLibreria.Adapters.Venta
{
    public interface IVentaAdapter
    {
        public interface IVentaAdapter
        {
            Task<List<VentaDTO>> CargarVentasAsync();
            Task<bool> RegistrarVentaAsync(RegistrarVentaRequestDTO request);
            Task<bool> AnularVentaAsync(int idVenta, int idEmpleado);
            Task<List<PresentacionProductoVentaDTO>> ObtenerPresentacionesPorFraseAsync(string frase);
            Task<PresentacionProductoVentaDTO?> ObtenerPresentacionProductoByIdsAsync(int idProducto, int idPresentacion);
            Task<byte[]> GenerarComprobantePdfAsync(int idVenta);
            Task<VentaCompletaDTO?> ObtenerVentaCompletaAsync(int idVenta);
        }
    }
}
