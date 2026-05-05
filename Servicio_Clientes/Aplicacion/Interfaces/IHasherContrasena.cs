namespace Servicio_Clientes.Aplicacion.Interfaces
{
    public interface IHasherContrasena
    {
        string Encriptar(string contrasena);
        bool Verificar(string contrasena, string contrasenaHasheada);
    }
}
