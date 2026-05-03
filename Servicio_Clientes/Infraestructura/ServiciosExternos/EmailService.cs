using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Servicio_Clientes.Aplicacion.Interfaces;

namespace Servicio_Clientes.Infraestructura.ServiciosExternos
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Para evitar problemas de certificados en desarrollo (opcional)
                // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(
                    smtpSettings["Server"],
                    int.Parse(smtpSettings["Port"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
