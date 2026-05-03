using LibreriaJoelitoV2.Aplicacion.Interfaces;

namespace LibreriaJoelitoV2.Infraestructura.FactoryCreators
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepository<T> CreateRepository();
    }
}
using LibreriaJoelitoV2.Aplicacion.Interfaces;
using LibreriaJoelitoV2.Dominio.Models;
using LibreriaJoelitoV2.Infraestructura.Persistencia.FactoryProducts;

namespace LibreriaJoelitoV2.Infraestructura.FactoryCreators
{
    public class ClienteCreatorRepository : CreatorRepository<Cliente>
    {
        public override IRepository<Cliente> CreateRepository()
        {
            return new ClienteRepository();
        }
    }
}
