using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Servicio_Clientes.Aplicacion.Interfaces;

namespace Servicio_Clientes.Infraestructura.ServiciosExternos
{
    public class ServicioEmail : IServicioEmail
    {
        private readonly IConfiguration _config;

        public ServicioEmail(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string para, string asunto, string cuerpo)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Librería Joelito", _config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(para));
            email.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = cuerpo };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
