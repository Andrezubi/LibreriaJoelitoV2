
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infraestructura.FactoriaCreadores;
using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;

namespace Servicio_Ventas.Infrestructura.FactoriaCreadores
{
    public class PresentacionProductoCreadorRepositorio:CreadorRepositorio<PresentacionProducto>
    {
        public override PresentacionProductoRepositorio CrearRepositorio()
        {
            return new PresentacionProductoRepositorio();
        }
    }
}
