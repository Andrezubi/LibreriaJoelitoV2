namespace Servicio_Clientes.Dominio.Models
{
    public class ModeloBase
    {
        public int Id { get; set; }
        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaUltimaActualizacion { get; set; }

        public int? IdEmpleadoCambio { get; set; }

        ModeloBase() { }
        ModeloBase(int id) { Id = id; }
    }
}