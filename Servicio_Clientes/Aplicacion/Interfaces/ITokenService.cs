namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(string username, string rol, string userId);
    }
}
