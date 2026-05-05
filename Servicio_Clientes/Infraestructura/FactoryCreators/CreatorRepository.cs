using Servicio_Clientes.Dominio.Interfaces;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public abstract class CreadorRepositorio<T>
    {
        public abstract IRepositorio<T> CrearRepositorio();
    }
}
