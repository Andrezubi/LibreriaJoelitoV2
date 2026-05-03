using Servicio_Clientes.Aplicacion.Interfaces;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepository<T> CreateRepository();
    }
}
