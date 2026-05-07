using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;

namespace Servicio_Clientes.Infraestructura.FactoryCreators
{
    public class ReporteCreadorRepositorio
    {
        public ReporteRepositorio CrearRepositorio()
        {
            return new ReporteRepositorio();
        }
    }
}
