using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infraestructura.FactoriaCreadores;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;

namespace Servicio_Ventas.Infrestructura.FactoriaCreadores
{
    public class VentaCreatorRepository : CreatorRepository<Venta>
    {
        public override VentaRepository CreateRepository()
        {
            return new VentaRepository();
        }
    }
}
