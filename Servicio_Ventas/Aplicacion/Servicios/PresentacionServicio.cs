
using LibreriaJoelito.Infraestructura.Persistencia.FactoryProducts;
using Servicio_Ventas.Dominio.Modelos;
using System.Data;

namespace Servicio_Ventas.Aplicacion.Servicios
{
    public class PresentacionServicio
    {
        private readonly PresentacionRepositorio _presentacionRepo;


        public PresentacionServicio(PresentacionRepositorio presentacionRepo)
        {
            _presentacionRepo = presentacionRepo;
        }

        public List<Presentacion> ObtenerTodo()
        {
            return _presentacionRepo.ObtenerTodo();
        }
    }
}
