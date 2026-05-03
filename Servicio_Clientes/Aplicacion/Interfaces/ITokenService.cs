namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(string username, string rol, string userId);
    }
}
