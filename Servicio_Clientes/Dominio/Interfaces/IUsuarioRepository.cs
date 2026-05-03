using LibreriaJoelitoV2.Dominio.Models;
using System.Data;

namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        bool ExisteUsername(string username);
        string GetPasswordByUsername(string username);

        Usuario GetDatosLogin(string username);


    }
}
