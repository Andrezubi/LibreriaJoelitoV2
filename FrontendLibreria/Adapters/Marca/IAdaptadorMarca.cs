using FrontendLibreria.DTOs;

namespace FrontendLibreria.Adapters.Marca
{
    public interface IAdaptadorMarca
    {
            Task<List<MarcaDto>> ObtenerTodoAsync();
            Task<ResultadoApi> InsertarAsync(MarcaDto marca);
            Task<ResultadoApi> ActualizarAsync(MarcaDto marca);
            Task<ResultadoApi> EliminarAsync(int id, int idUsuario);
    }
}
