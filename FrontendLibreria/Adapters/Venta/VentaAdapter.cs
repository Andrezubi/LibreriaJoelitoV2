using FrontendLibreria.DTOs.VentaDTOs;
using System.Net.Http.Json;

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
            var ventas = await _httpClient.GetFromJsonAsync<List<VentaDTO>>("api/Venta");
            return ventas ?? new List<VentaDTO>();
        }

        public async Task<ApiResultDTO<int>?> RegistrarVentaAsync(RegistrarVentaRequestDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Venta", request);
            return await response.Content.ReadFromJsonAsync<ApiResultDTO<int>>();
        }

        public async Task<ApiResultDTO<int>?> AnularVentaAsync(int idVenta, int idEmpleado)
        {
            var response = await _httpClient.PutAsync(
                $"api/Venta/{idVenta}/anular?idEmpleado={idEmpleado}",
                null
            );

            return await response.Content.ReadFromJsonAsync<ApiResultDTO<int>>();
        }

        public async Task<List<PresentacionProductoVentaDTO>> ObtenerPresentacionesPorFraseAsync(string frase)
        {
            var resultado = await _httpClient.GetFromJsonAsync<List<PresentacionProductoVentaDTO>>(
                $"api/Venta/presentaciones?frase={Uri.EscapeDataString(frase)}"
            );

            return resultado ?? new List<PresentacionProductoVentaDTO>();
        }

        public async Task<PresentacionProductoVentaDTO?> ObtenerPresentacionProductoByIdsAsync(
            int idProducto,
            int idPresentacion)
        {
            return await _httpClient.GetFromJsonAsync<PresentacionProductoVentaDTO>(
                $"api/Venta/productos/{idProducto}/presentaciones/{idPresentacion}"
            );
        }

        public async Task<byte[]> GenerarComprobantePdfAsync(int idVenta)
        {
            return await _httpClient.GetByteArrayAsync(
                $"api/Venta/{idVenta}/comprobante"
            );
        }

        public async Task<VentaCompletaDTO?> ObtenerVentaCompletaAsync(int idVenta)
        {
            return await _httpClient.GetFromJsonAsync<VentaCompletaDTO>(
                $"api/Venta/{idVenta}/completa"
            );
        }
    }
}