using System.Collections.Generic;

namespace FrontendLibreria.DTOs.Servicio2DTOs
{
    public class RespuestaErrorApi
    {
        public List<string> Errores { get; set; } = new();
        public string? Mensaje { get; set; }
    }
}
