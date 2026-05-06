namespace Servicio_Clientes.Dominio.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Ci { get; set; }
        public string? Complemento { get; set; }
        public string? DireccionDomicilio { get; set; }
        public string Email { get; set; }
        public string? Telefono { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public DateOnly FechaIngreso { get; set; }

        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
        public int IdUsuario { get; set; }

        public bool DebeCambiarContrasena { get; set; }
        public bool Estado { get; set; }
        public int? IdEmpleadoCambio { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }



        public Usuario() { }
        public Usuario(int id) { Id = id; }

        public Usuario(int id, string nombre, string apellidoPaterno, string apellidoMaterno, string ci, string complemento, string direccionDomicilio, string email, string telefono, DateOnly fechaNacimiento, DateOnly fechaIngreso, string nombreUsuario, string contrasena, string rol)
        {
            Id = id;
            Nombre = nombre;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            Ci = ci;
            Complemento = complemento;
            DireccionDomicilio = direccionDomicilio;
            Email = email;
            Telefono = telefono;
            FechaNacimiento = fechaNacimiento;
            FechaIngreso = fechaIngreso;
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            Rol = rol;

        }

        public Usuario(string nombre, string apellidoPaterno, string apellidoMaterno, string ci, string complemento, string direccionDomicilio, string email, string telefono, DateOnly fechaNacimiento, DateOnly fechaIngreso, string nombreUsuario, string contrasena, string rol, int idusuario)
        {
            Nombre = nombre;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            Ci = ci;
            Complemento = complemento;
            DireccionDomicilio = direccionDomicilio;
            Email = email;
            Telefono = telefono;
            FechaNacimiento = fechaNacimiento;
            FechaIngreso = fechaIngreso;
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            Rol = rol;
            IdUsuario = idusuario;
        }
    }
}