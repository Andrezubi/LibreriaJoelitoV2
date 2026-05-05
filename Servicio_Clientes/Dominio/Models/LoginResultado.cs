namespace Servicio_Clientes.Dominio.Models
{
    public class LoginResultado
    {
        public bool Exito { get; set; }
        public string Token { get; set; }
        public string Mensaje { get; set; }
        public string Rol { get; set; }
        public bool DebeCambiarContrasena { get; set; }
    }
}
