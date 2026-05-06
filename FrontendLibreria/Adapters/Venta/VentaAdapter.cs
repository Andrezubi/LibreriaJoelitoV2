using FrontendLibreria.DTOs.VentaDTOs;

namespace FrontendLibreria.Adapters.Venta
{
    public class VentaAdapter : IVentaAdapter
    {
        private readonly HttpClient _httpClient;

        public VentaAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<VentaDTO>> CargarVentasAsync()
        {
            var ventas = await _httpClient.GetFromJsonAsync<List<VentaDTO>>("api/venta");
            return ventas ?? new List<VentaDTO>();
        }

        public async Task<bool> RegistrarVentaAsync(RegistrarVentaRequestDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/venta", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AnularVentaAsync(int idVenta, int idEmpleado)
        {
            var response = await _httpClient.PutAsync(
                $"api/venta/{idVenta}/anular?idEmpleado={idEmpleado}",
                null
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<List<PresentacionProductoVentaDTO>> ObtenerPresentacionesPorFraseAsync(string frase)
        {
            var resultado = await _httpClient.GetFromJsonAsync<List<PresentacionProductoVentaDTO>>(
                $"api/venta/presentaciones?frase={Uri.EscapeDataString(frase)}"
            );

            return resultado ?? new List<PresentacionProductoVentaDTO>();
        }

        public async Task<PresentacionProductoVentaDTO?> ObtenerPresentacionProductoByIdsAsync(
            int idProducto,
            int idPresentacion)
        {
            return await _httpClient.GetFromJsonAsync<PresentacionProductoVentaDTO>(
                $"api/venta/productos/{idProducto}/presentaciones/{idPresentacion}"
            );
        }

        public async Task<byte[]> GenerarComprobantePdfAsync(int idVenta)
        {
            return await _httpClient.GetByteArrayAsync(
                $"api/venta/{idVenta}/comprobante"
            );
        }

        public async Task<VentaCompletaDTO?> ObtenerVentaCompletaAsync(int idVenta)
        {
            return await _httpClient.GetFromJsonAsync<VentaCompletaDTO>(
                $"api/venta/{idVenta}/completa"
            );
        }
    }
}
