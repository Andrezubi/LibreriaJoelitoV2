using Servicio_Clientes.Dominio.Models;
using System.Data;


namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        // no estoy segura de usar estos métodos, creo que sería mejor borrar la clase
        List<T> GetByCi(string ci);
        List<T> GetAllSimilarId(string ci);
    }
}
