using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        bool ExisteUsername(string username);
        string GetPasswordByUsername(string username);

        Usuario GetDatosLogin(string username);


    }
}
