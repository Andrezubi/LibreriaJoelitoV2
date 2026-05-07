using FrontendLibreria.DTOs.Servicio2DTOs;

namespace FrontendLibreria.Adapters.Servicio2Adapters
{
    public class UsuarioServicioAdapter : IUsuarioServicioAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioServicioAdapter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioServicioAdapter(HttpClient httpClient, ILogger<UsuarioServicioAdapter> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            // Extraer el IdUsuario de los Claims de la sesión actual
            var idUsuario = _httpContextAccessor.HttpContext?.User?.FindFirst("IdUsuario")?.Value;
            if (!string.IsNullOrEmpty(idUsuario))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-IdUsuario"))
                    _httpClient.DefaultRequestHeaders.Remove("X-IdUsuario");

                _httpClient.DefaultRequestHeaders.Add("X-IdUsuario", idUsuario);
            }

            // Extraer el Token JWT y enviarlo en la cabecera Authorization
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
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

        public async Task<List<UsuarioDto>> ObtenerTodos()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/usuarios");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UsuarioDto>>() ?? new List<UsuarioDto>();
                }
                return new List<UsuarioDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return new List<UsuarioDto>();
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/usuarios/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {Id}", id);
                return false;
            }
        }

        public async Task<(bool Exito, List<string> Errores)> Insertar(SolicitudCrearUsuarioDto request)
        {
            try
            {
                // Asegurar formato de fecha para el backend (YYYY-MM-DD)
                string fechaNac = request.FechaNacimiento ?? "";
                if (DateTime.TryParse(fechaNac, out var fechaParsed))
                {
                    fechaNac = fechaParsed.ToString("yyyy-MM-dd");
                }

                // El backend espera un objeto Usuario completo, agregamos campos técnicos faltantes
                var payload = new
                {
                    Nombre = request.Nombre,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Ci = request.Ci,
                    Complemento = request.Complemento,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    Rol = request.Rol,
                    DireccionDomicilio = request.DireccionDomicilio ?? "Dirección no especificada",
                    FechaNacimiento = fechaNac,
                    FechaIngreso = DateTime.Now.ToString("yyyy-MM-dd"),
                    IdUsuario = 1 // ID por defecto del admin
                };

                var response = await _httpClient.PostAsJsonAsync("/api/usuarios", payload);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, new List<string>());
                }

                var content = await response.Content.ReadAsStringAsync();
                var errores = new List<string>();

                try 
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    // 1. Intentar formato personalizado { "errores": [...] }
                    if (root.TryGetProperty("errores", out var erroresProp) && erroresProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var err in erroresProp.EnumerateArray())
                            errores.Add(err.GetString() ?? "");
                    }
                    // 2. Intentar formato estándar ASP.NET { "errors": { "Campo": ["Error"] } }
                    else if (root.TryGetProperty("errors", out var validationErrors) && validationErrors.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var prop in validationErrors.EnumerateObject())
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                                errores.Add($"{prop.Name}: {err.GetString()}");
                        }
                    }
                }
                catch 
                {
                    errores.Add("Error de validación en los datos. Verifique CI, Email y que sea mayor de 18 años.");
                }

                return (false, errores.Any() ? errores : new List<string> { "Error desconocido en el servidor." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar usuario");
                return (false, new List<string> { $"Error crítico: {ex.Message}" });
            }
        }

        public async Task<UsuarioDto?> ObtenerPorId(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/usuarios/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por id {Id}", id);
                return null;
            }
        }

        public async Task<(bool Exito, List<string> Errores)> Actualizar(int id, SolicitudCrearUsuarioDto request)
        {
            try
            {
                string fechaNac = request.FechaNacimiento ?? "";
                if (DateTime.TryParse(fechaNac, out var fechaParsed))
                {
                    fechaNac = fechaParsed.ToString("yyyy-MM-dd");
                }

                var payload = new
                {
                    Nombre = request.Nombre,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Ci = request.Ci,
                    Complemento = request.Complemento,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    Rol = request.Rol,
                    DireccionDomicilio = request.DireccionDomicilio ?? "Dirección no especificada",
                    FechaNacimiento = fechaNac
                };

                var response = await _httpClient.PutAsJsonAsync($"/api/usuarios/{id}", payload);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, new List<string>());
                }

                var content = await response.Content.ReadAsStringAsync();
                var errores = new List<string>();

                try 
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("errores", out var erroresProp) && erroresProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var err in erroresProp.EnumerateArray())
                            errores.Add(err.GetString() ?? "");
                    }
                    else if (root.TryGetProperty("errors", out var validationErrors) && validationErrors.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var prop in validationErrors.EnumerateObject())
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                                errores.Add($"{prop.Name}: {err.GetString()}");
                        }
                    }
                }
                catch 
                {
                    errores.Add("Error de validación en los datos.");
                }

                return (false, errores.Any() ? errores : new List<string> { "Error desconocido en el servidor." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return (false, new List<string> { $"Error crítico: {ex.Message}" });
            }
        }
    }
}
