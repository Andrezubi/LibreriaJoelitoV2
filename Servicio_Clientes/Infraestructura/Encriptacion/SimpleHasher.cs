using Servicio_Clientes.Aplicacion.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Servicio_Clientes.Infraestructura.Encryptacion
{
    public class SimpleHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return Hash(password) == hashedPassword;
        }
    }
}
