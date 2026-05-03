using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Aplicacion.Interfaces
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
