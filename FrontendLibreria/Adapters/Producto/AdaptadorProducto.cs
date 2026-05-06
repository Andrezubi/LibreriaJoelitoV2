using FrontendLibreria.DTOs;
using Microsoft.AspNetCore.Http;

namespace FrontendLibreria.Adapters.Producto
{
    // Adapters/ProductoAdapter.cs
    public class AdaptadorProducto : IAdaptadorProducto
    {
        private readonly HttpClient _http;

        public AdaptadorProducto(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductoDto>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/productos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProductoDto>>() ?? new();
        }

        public async Task<List<CategoriaDto>> GetCategoriasAsync()
            => await _http.GetFromJsonAsync<List<CategoriaDto>>("api/productos/categorias") ?? new();

        public async Task<List<MarcaDto>> GetMarcasAsync()
            => await _http.GetFromJsonAsync<List<MarcaDto>>("api/productos/marcas") ?? new();

        public async Task<List<PresentacionDto>> GetPresentacionesAsync()
            => await _http.GetFromJsonAsync<List<PresentacionDto>>("api/productos/presentaciones") ?? new();

        public async Task<ResultadoApi> UpdateAsync(ProductoDto producto)
        {
            var response = await _http.PutAsJsonAsync($"api/productos/{producto.Id}", producto);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }

        public async Task<ResultadoApi> DeleteAsync(int id, int idUsuario)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/productos/{id}")
            {
                Content = JsonContent.Create(new { IdUsuario = idUsuario })
            };
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode ? ResultadoApi.Ok() : ResultadoApi.Fail(new List<string> { "Error al eliminar" });
        }

        public async Task<ResultadoApi> AgregarPresentacionAsync(SolicitudAgregarPresentacion request)
        {
            var response = await _http.PostAsJsonAsync($"api/productos/{request.IdProducto}/presentaciones", request);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }
    }
}
