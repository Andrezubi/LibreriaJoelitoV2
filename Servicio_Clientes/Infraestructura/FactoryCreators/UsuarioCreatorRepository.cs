using LibreriaJoelitoV2.Aplicacion.Interfaces;
using LibreriaJoelitoV2.Dominio.Models;
using LibreriaJoelitoV2.Infraestructura.Persistencia.FactoryProducts;

namespace LibreriaJoelitoV2.Infraestructura.FactoryCreators
{
    public class UsuarioCreatorRepository : CreatorRepository<Usuario>
    {
        public override IRepository<Usuario> CreateRepository()
        {
            return new UsuarioRepository();
        }
    }
}
