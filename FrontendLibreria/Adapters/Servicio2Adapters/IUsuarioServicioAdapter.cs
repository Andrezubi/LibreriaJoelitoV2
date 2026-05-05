using FrontendLibreria.DTOs.Servicio2DTOs;

namespace FrontendLibreria.Adapters.Servicio2Adapters
{
    public interface IUsuarioServicioAdapter
    {
        Task<ResultLoginDto> Login(SolicitudLoginDto request);
    }
}
