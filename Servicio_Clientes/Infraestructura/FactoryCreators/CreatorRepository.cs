using LibreriaJoelitoV2.Aplicacion.Interfaces;

namespace LibreriaJoelitoV2.Infraestructura.FactoryCreators
{
    public abstract class CreatorRepository<T>
    {
        public abstract IRepository<T> CreateRepository();
    }
}
