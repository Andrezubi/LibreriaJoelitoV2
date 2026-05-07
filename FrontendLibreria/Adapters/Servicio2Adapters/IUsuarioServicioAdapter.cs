using FrontendLibreria.DTOs.Servicio2DTOs;

namespace FrontendLibreria.Adapters.Servicio2Adapters
{
    public interface IUsuarioServicioAdapter
    {
        Task<ResultLoginDto> Login(SolicitudLoginDto request);
        Task<bool> EsUsuarioEliminado(string nombreUsuario);
        Task<List<UsuarioDto>> ObtenerTodos();
        Task<bool> Eliminar(int id);
        Task<(bool Exito, List<string> Errores)> Insertar(SolicitudCrearUsuarioDto request);
        Task<byte[]?> GenerarReporteVentasCategoria(
    DateTime fechaDesde, DateTime fechaHasta, string format, string token);
        Task<(bool Exito, List<string> Errores)> CambiarContrasena(
    string contrasenaActual,
    string nuevaContrasena,
    string confirmacion,
    string token);
    }
}
