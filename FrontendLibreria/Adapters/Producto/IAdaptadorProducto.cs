using FrontendLibreria.DTOs;
using Microsoft.AspNetCore.Http;

namespace FrontendLibreria.Adapters.Producto
{
    public interface IAdaptadorProducto
    {
        Task<List<ProductoDto>> GetAllAsync();
        Task<List<CategoriaDto>> GetCategoriasAsync();
        Task<List<MarcaDto>> GetMarcasAsync();
        Task<List<PresentacionDto>> GetPresentacionesAsync();
        Task<ResultadoApi> UpdateAsync(ProductoDto producto);
        Task<ResultadoApi> DeleteAsync(int id, int idUsuario);
        Task<ResultadoApi> AgregarPresentacionAsync(SolicitudAgregarPresentacion request);
    }
}
