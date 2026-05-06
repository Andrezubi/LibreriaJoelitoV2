

using LibreriaJoelito.Infraestructura.Persistencia.FactoryProducts;
using Servicio_Ventas.Aplicacion.Interfaces;
using Servicio_Ventas.Dominio.Modelos;
using Servicio_Ventas.Infraestructura.FactoriaCreadores;

namespace Servicio_Ventas.Infrestructura.FactoriaCreadores 
{ 
    public class ProductoCreadorRepositorio:CreadorRepositorio<Producto>
    {
        public override IRepositorio<Producto> CrearRepositorio()
        {
            return new ProductoRepositorio();
        }
    }
}
