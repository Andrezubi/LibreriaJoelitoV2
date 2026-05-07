using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public class UsuarioCreadorRepositorio : CreadorRepositorio<Usuario>
    {
        public override UsuarioRepository CrearRepositorio()
        {
            return new UsuarioRepository();
        }
    }
}
