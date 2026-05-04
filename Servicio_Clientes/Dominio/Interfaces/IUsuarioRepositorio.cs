using Servicio_Clientes.Dominio.Models;

namespace Servicio_Clientes.Dominio.Interfaces
{
    public interface IUsuarioRepositorio
    {
        bool ExisteUsername(string nombreUsuario);
        string ObtenerContrasenaPorNombreUsuario(string nombreUsuario);
        Usuario ObtenerDatosLogin(string nombreUsuario);
    }
}
