namespace LibreriaJoelitoV2.Aplicacion.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
