namespace FrontendLibreria.DTOs.Servicio2DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string? ApellidoMaterno { get; set; }
        public string Ci { get; set; } = "";
        public string? Complemento { get; set; }
        public string? DireccionDomicilio { get; set; }
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }
        public string Rol { get; set; } = "";
        public string NombreUsuario { get; set; } = "";
        public bool Estado { get; set; }
    }

    public class SolicitudCrearUsuarioDto
    {
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string? ApellidoMaterno { get; set; }
        public string Ci { get; set; } = "";
        public string? Complemento { get; set; }
        public string? DireccionDomicilio { get; set; }
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }
        public string Rol { get; set; } = "";
        public string? FechaNacimiento { get; set; }
    }
}
