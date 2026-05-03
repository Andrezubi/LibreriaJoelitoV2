using LibreriaJoelito.Dominio.Models;
using System.Data;

namespace LibreriaJoelito.Aplicacion.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        bool ExisteUsername(string username);
        string GetPasswordByUsername(string username);

        Usuario GetDatosLogin(string username);


    }
}
