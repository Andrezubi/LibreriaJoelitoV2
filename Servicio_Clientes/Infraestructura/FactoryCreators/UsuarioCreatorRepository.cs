using Servicio_Clientes.Dominio.Interfaces;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public class UsuarioCreatorRepository : CreatorRepository<Usuario>
    {
        public override IRepositorio<Usuario> CreateRepository()
        {
            return new UsuarioRepository();
        }
    }
}
