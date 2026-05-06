using FrontendLibreria.DTOs;

namespace FrontendLibreria.Adapters.Cliente
{
    public class AdaptadorCliente : IAdaptadorCliente
    {
        private readonly HttpClient _http;

        public AdaptadorCliente(HttpClient http)
        {
            _http = http;
        }
        public async Task<ResultadoApi> InsertarAsync(ClienteDto cliente)
        {
            var response = await _http.PostAsJsonAsync("api/clientes", cliente);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }
        public async Task<List<ClienteDto>> ObtenerTodoAsync()
        {
            var response = await _http.GetAsync("api/clientes");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ClienteDto>>() ?? new();
        }

        public async Task<ResultadoApi> ActualizarAsync(ClienteDto cliente)
        {
            var response = await _http.PutAsJsonAsync($"api/clientes/{cliente.Id}", cliente);
            if (response.IsSuccessStatusCode) return ResultadoApi.Ok();
            var error = await response.Content.ReadFromJsonAsync<RespuestaErrorApi>();
            return ResultadoApi.Fail(error?.Errores ?? new List<string> { "Error desconocido" });
        }

        public async Task<ResultadoApi> EliminarAsync(int id, int idUsuario)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/clientes/{id}")
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