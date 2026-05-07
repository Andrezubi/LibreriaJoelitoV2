namespace Servicio_Clientes.Dominio.Models
{
    public class CambiarContrasenaPeticion
    {
        public string ContrasenaActual { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
        public string ConfirmacionContrasena { get; set; } = string.Empty;
    }
}
