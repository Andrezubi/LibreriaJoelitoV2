using FrontendLibreria.DTOs;
using System.Net.Http.Json;

namespace FrontendLibreria.Adapters.Marca
{
    public class AdaptadorMarca : IAdaptadorMarca
    {
        private readonly HttpClient _http;

        public AdaptadorMarca(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MarcaDto>> ObtenerTodoAsync()
        {
            var response = await _http.GetAsync("api/Marca");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MarcaDto>>() ?? new();
        }

        public async Task<ResultadoApi> InsertarAsync(MarcaDto marca)
        {
            var response = await _http.PostAsJsonAsync("api/Marca", marca);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }

        public async Task<ResultadoApi> ActualizarAsync(MarcaDto marca)
        {
            var response = await _http.PutAsJsonAsync($"api/Marca/{marca.Id}", marca);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }

        public async Task<ResultadoApi> EliminarAsync(int id, int idUsuario)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/Marca/{id}")
            {
                Content = JsonContent.Create(new { IdUsuario = idUsuario })
            };
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode
                ? ResultadoApi.Ok()
                : ResultadoApi.Fail(new List<string> { "Error al eliminar" });
        }
    }
}