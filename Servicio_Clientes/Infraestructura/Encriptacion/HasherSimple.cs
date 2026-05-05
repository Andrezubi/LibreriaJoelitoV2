using Servicio_Clientes.Aplicacion.Interfaces;

namespace Servicio_Clientes.Infraestructura.Encryptacion
{
    public class HasherSimple : IHasherContrasena
    {
        public string Encriptar(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public bool Verificar(string contrasena, string contrasenaHasheada)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, contrasenaHasheada);
        }
    }
}
