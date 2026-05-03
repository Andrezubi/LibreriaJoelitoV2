using LibreriaJoelitoV2.Dominio.Models;
using System.Data;

namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IUsuarioService
    {
        int InsertUsuario(Usuario usuario);

        int UpdateUsuario(Usuario usuario);

        int DeleteUsuario(Usuario t);

        List<T> GetAllUsuarios();
        // List<T> GetUsuarioById(int id); no getById

        bool ExisteUsuarioDuplicado(Usuario usuario);
        string GenerarUsername(string nombre, string apellido);
        string GenerarPassword(int length);

        LoginResult Login(string username, string password);

    }
}
