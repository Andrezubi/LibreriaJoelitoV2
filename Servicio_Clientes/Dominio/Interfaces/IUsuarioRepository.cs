using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Dominio.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        bool ExisteUsername(string username);
        string GetPasswordByUsername(string username);

        Usuario GetDatosLogin(string username);


    }
}
