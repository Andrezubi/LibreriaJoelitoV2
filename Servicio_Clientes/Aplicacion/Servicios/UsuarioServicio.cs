using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Aplicacion.Results;
using Servicio_Clientes.Aplicacion.Validators;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using System.Data;

namespace Servicio_Clientes.Aplicacion.Servicios
{
    public class UsuarioServicio
    {
        private readonly UsuarioRepository _usuarioRepositorio;
        private readonly IHasherContrasena _encriptador;
        private readonly IServicioToken _servicioToken;
        private readonly IServicioEmail _servicioEmail;

        public UsuarioServicio(UsuarioRepository usuarioRepositorio, IHasherContrasena encriptador, IServicioToken servicioToken, IServicioEmail servicioEmail)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encriptador = encriptador;
            _servicioToken = servicioToken;
            _servicioEmail = servicioEmail;
        }

        public DataTable ObtenerTodo()
        {
            return _usuarioRepositorio.ObtenerTodo();
        }

        public DataRow? ObtenerPorId(int id)
        {
            return _usuarioRepositorio.ObtenerPorId(id);
        }

        public async Task<Resultado> InsertarAsync(Usuario usuario)
        {
            var validaciones = ValidadorEmpleado.Validar(usuario);

            if (validaciones.Any())
            {
                var errores = validaciones
                    .Select(v =>
                    {
                        var field = v.MemberNames.FirstOrDefault() ?? "General";
                        return $"{field}: {v.ErrorMessage}";
                    })
                    .ToList();

                return Resultado.Failure(errores);
            }

            if (_usuarioRepositorio.ExisteDuplicado(usuario))
            {
                return Resultado.Failure("empleado.Ci: El empleado con ese CI ya existe.");
            }

            // Establecer valores por defecto si no vienen del frontend
            if (usuario.FechaIngreso == default)
                usuario.FechaIngreso = DateOnly.FromDateTime(DateTime.Today);
            
            if (usuario.IdUsuario == 0)
                usuario.IdUsuario = 1; // ID del administrador por defecto

            // Generar credenciales
            string nombreUsuarioPlano = GenerarNombreUsuario(usuario.Nombre, usuario.ApellidoPaterno);
            string contrasenaPlana = GenerarContrasena(10); // Generar contraseña de 10 caracteres
            
            usuario.NombreUsuario = nombreUsuarioPlano;
            usuario.DebeCambiarContrasena = true;

            // Hashear la contraseña antes de guardar en la DB
            usuario.Contrasena = _encriptador.Encriptar(contrasenaPlana);
            
            _usuarioRepositorio.Insertar(usuario);

            // Enviar credenciales por correo de forma asíncrona (fire and forget o awaited)
            string asunto = "Bienvenido a Librería Joelito - Sus Credenciales";
            string cuerpo = $@"
                <h3>Bienvenido/a {usuario.Nombre} {usuario.ApellidoPaterno}</h3>
                <p>Se ha creado su cuenta en el sistema de Librería Joelito.</p>
                <p><strong>Usuario:</strong> {nombreUsuarioPlano}</p>
                <p><strong>Contraseña Temporal:</strong> {contrasenaPlana}</p>
                <p><em>Por motivos de seguridad, el sistema le pedirá cambiar esta contraseña en su primer inicio de sesión.</em></p>
            ";

            try
            {
                await _servicioEmail.EnviarCorreoAsync(usuario.Email, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                // Aquí se podría registrar el log del error de envío de correo
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
                // No detenemos la creación del usuario si el correo falla, o podríamos retornar un warning.
            }

            return Resultado.Success();
        }

        public Resultado Actualizar(Usuario usuario)
        {
            var validaciones = ValidadorEmpleado.Validar(usuario);

            if (validaciones.Any())
            {
                var errores = validaciones
                    .Select(v =>
                    {
                        return $"{v.ErrorMessage}";
                    })
                    .ToList();

                return Resultado.Failure(errores);
            }

            _usuarioRepositorio.Actualizar(usuario);

            return Resultado.Success();
        }

        public int Eliminar(Usuario usuario)
        {
            return _usuarioRepositorio.Eliminar(usuario);
        }

        public string GenerarNombreUsuario(string nombre, string apellidoPaterno)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellidoPaterno))
                return string.Empty;

            string primeraLetra = nombre.Trim().Substring(0, 1).ToLower();
            string apellidoLimpio = apellidoPaterno.Trim().ToLower().Replace(" ", "");
            
            string baseUsername = $"{primeraLetra}{apellidoLimpio}";
            string username = baseUsername;
            int counter = 1;

            while (_usuarioRepositorio.ExisteUsername(username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }

        public string GenerarContrasena(int longitud = 8)
        {
            if (longitud < 8) longitud = 8; // Mínimo 8 caracteres

            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string symbols = "!@#$%^&*";

            var random = new Random();
            var password = new char[longitud];

            // Garantizar al menos un carácter de cada tipo
            password[0] = uppercase[random.Next(uppercase.Length)];
            password[1] = lowercase[random.Next(lowercase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = symbols[random.Next(symbols.Length)];

            string allChars = uppercase + lowercase + numbers + symbols;

            // Rellenar el resto de forma aleatoria
            for (int i = 4; i < longitud; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Mezclar los caracteres
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }

        public LoginResultado Login(string nombreUsuario, string contrasena)
        {
            var loginResultado = new LoginResultado();
            var user = _usuarioRepositorio.ObtenerDatosLogin(nombreUsuario);

            if (user == null)
            {
                loginResultado.Exito = false;
                loginResultado.Mensaje = "Usuario no encontrado.";
                loginResultado.Token = null;
            }
            else if (_encriptador.Verificar(contrasena, user.Contrasena))
            {
                loginResultado.Exito = true;
                loginResultado.Mensaje = "Acceso concedido.";
                loginResultado.Token = _servicioToken.GenerarToken(nombreUsuario, user.Rol, user.Id.ToString());
                loginResultado.DebeCambiarContrasena = user.DebeCambiarContrasena;
                loginResultado.Rol = user.Rol;
                loginResultado.IdUsuario = user.Id;
            }
            else
            {
                loginResultado.Exito = false;
                loginResultado.Mensaje = "Contraseña incorrecta.";
                loginResultado.Token = null;
            }

            return loginResultado;
        }

        public bool EstadoUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return false;
            var usuario = _usuarioRepositorio.ObtenerDatosLogin(nombreUsuario);
            // Si usuario es null -> no existe o está inactivo -> considerarlo eliminado = true
            return usuario == null;
        }
    }
}
