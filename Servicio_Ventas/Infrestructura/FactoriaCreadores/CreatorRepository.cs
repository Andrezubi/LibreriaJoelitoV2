using Servicio_Ventas.Aplicacion.Interfaces;

namespace Servicio_Ventas.Infraestructura.FactoriaCreadores
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepository<T> CreateRepository();
    }
}
