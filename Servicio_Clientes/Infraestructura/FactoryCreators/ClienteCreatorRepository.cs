
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
