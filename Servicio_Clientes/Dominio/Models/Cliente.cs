namespace LibreriaJoelitoV2.Dominio.Models
{
    public class Cliente : Persona
    {

        public bool ClienteFrecuente { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public int? IdUsuario { get; set; }

        public Cliente() { }

        public Cliente(string nombre, string apellidoPaterno, string? apellidoMaterno, string cI, string? complemento, string? email, bool clienteFrecuente)
            : base(nombre, apellidoPaterno, apellidoMaterno, cI, complemento, email)
        {
            ClienteFrecuente = clienteFrecuente;
        }
    }
}