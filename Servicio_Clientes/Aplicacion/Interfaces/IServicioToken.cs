namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IServicioToken
    {
        string GenerarToken(string nombreUsuario, string rol, string id);
    }
}
