using Servicio_Clientes.Aplicacion.Interfaces;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepository<T> CreateRepository();
    }
}
using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public class ClienteCreatorRepository : CreatorRepository<Cliente>
    {
        public override IRepository<Cliente> CreateRepository()
        {
            return new ClienteRepository();
        }
    }
}
