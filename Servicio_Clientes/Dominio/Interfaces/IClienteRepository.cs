using LibreriaJoelitoV2.Dominio.Models;
using System.Data;


namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        // no estoy segura de usar estos métodos, creo que sería mejor borrar la clase
        List<T> GetByCi(string ci);
        List<T> GetAllSimilarId(string ci);
    }
}
