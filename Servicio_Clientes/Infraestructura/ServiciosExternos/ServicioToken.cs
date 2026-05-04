using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servicio_Clientes.Aplicacion.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Servicio_Clientes.Infraestructura.ServiciosExternos
{
    public class ServicioToken : IServicioToken
    {
        private readonly IConfiguration _config;

        public ServicioToken(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(string nombreUsuario, string rol, string id)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, nombreUsuario),
                new Claim(ClaimTypes.Role, rol)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}