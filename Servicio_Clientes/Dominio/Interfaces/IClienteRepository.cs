using Servicio_Clientes.Dominio.Models;
using System.Data;


namespace Servicio_Clientes.Dominio.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        // no estoy segura de usar estos métodos, creo que sería mejor borrar la clase
        List<Cliente> GetByCi(string ci);
        List<Cliente> GetAllSimilarId(string ci);
    }
}
