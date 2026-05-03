namespace Servicio_Clientes.Dominio.Models
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

        public string Rol { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
