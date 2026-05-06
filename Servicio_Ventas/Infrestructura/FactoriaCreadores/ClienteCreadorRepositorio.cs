using Servicio_Ventas.Infrestructura.Persistencia.FactoriaProductos;
using MySqlX.XDevAPI;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infraestructura.FactoriaCreadores;

namespace Servicio_Ventas.Infrestructura.FactoriaCreadores
{
    public class ClienteCreadorRepositorio : CreadorRepositorio<Cliente>
    {
        public override ClienteRepositorio CrearRepositorio()
        {
            return new ClienteRepositorio();
        }
    }
}
