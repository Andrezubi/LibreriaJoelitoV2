using Servicio_Clientes.Dominio.Interfaces;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepositorio<T> CreateRepository();
    }
}
