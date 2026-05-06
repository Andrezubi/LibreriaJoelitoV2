using FrontendLibreria.DTOs.Servicio2DTOs;

namespace FrontendLibreria.Adapters.Servicio2Adapters
{
    public class UsuarioServicioAdapter : IUsuarioServicioAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioServicioAdapter> _logger;

        public UsuarioServicioAdapter(HttpClient httpClient, ILogger<UsuarioServicioAdapter> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ResultLoginDto> Login(SolicitudLoginDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResultLoginDto>();
                    return result ?? new ResultLoginDto { Exito = false, Mensaje = "Respuesta vacía del servidor." };
                }

                return new ResultLoginDto
                {
                    Exito = false,
                    Mensaje = "Credenciales incorrectas."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar con el Servicio de Usuarios");
                return new ResultLoginDto
                {
                    Exito = false,
                    Mensaje = "No se pudo conectar con el servicio de autenticación."
                };
            }
        }

    public async Task<bool> EsUsuarioEliminado(string nombreUsuario)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/usuarios/estado?nombreUsuario={nombreUsuario}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>();
                    return result;
                }
                _logger.LogWarning("No se pudo verificar el estado del usuario {NombreUsuario}. Código de respuesta: {StatusCode}", nombreUsuario, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el estado del usuario {NombreUsuario}", nombreUsuario);
                return false;
            }
        }
    }
}
