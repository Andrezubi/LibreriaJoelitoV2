using Servicio_Ventas.Aplicacion.Interfaces;

namespace Servicio_Ventas.Infraestructura.FactoriaCreadores
{
    public abstract class CreadorRepositorio<T>
    {
        public abstract IRepositorio<T> CrearRepositorio();
    }
}
