using FrontendLibreria.DTOs;
using Microsoft.AspNetCore.Http;
namespace FrontendLibreria.Adapters.Cliente
{
    public interface IAdaptadorCliente
    {
        Task<ResultadoApi> InsertarAsync(ClienteDto cliente);
        Task<List<ClienteDto>> ObtenerTodoAsync();
        Task<ResultadoApi> ActualizarAsync(ClienteDto cliente);
        Task<ResultadoApi> EliminarAsync(int id, int idUsuario);
    }
}
