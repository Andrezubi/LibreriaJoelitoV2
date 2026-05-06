

using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infraestructura.FactoriaCreadores;

namespace Servicio_Ventas.Infrestructura.FactoriaCreadores
{
    public class PresentacionCreadorRepositorio : CreadorRepositorio<Presentacion>
    {
        public override IRepositorio<Presentacion> CrearRepositorio()
        {
            return new PresentacionRepositorio();
        }
    }
}