namespace FrontendLibreria.DTOs.Servicio2DTOs
{
    public class ResultLoginDto
    {
        public bool Exito { get; set; }
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
