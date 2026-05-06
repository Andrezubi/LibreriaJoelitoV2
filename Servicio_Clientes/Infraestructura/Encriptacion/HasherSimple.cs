using Servicio_Clientes.Aplicacion.Interfaces;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;

namespace Servicio_Clientes.Infraestructura.Encriptacion
{
    public class HasherSimple : IHasherContrasena
    {
        public string Encriptar(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena, workFactor: 10);
        }

        public bool Verificar(string contrasena, string contrasenaHasheada)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, contrasenaHasheada);
        }
    }
}
