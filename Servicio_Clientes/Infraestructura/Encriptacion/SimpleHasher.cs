using Servicio_Clientes.Aplicacion.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Servicio_Clientes.Infraestructura.Encryptacion
{
    public class SimpleHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
