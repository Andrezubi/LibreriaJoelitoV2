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

        public UsuarioServicio(UsuarioRepository usuarioRepositorio, IHasherContrasena encriptador, IServicioToken servicioToken)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _encriptador = encriptador;
            _servicioToken = servicioToken;
        }

        public DataTable ObtenerTodo()
        {
            return _usuarioRepositorio.ObtenerTodo();
        }

        public DataRow? ObtenerPorId(int id)
        {
            return _usuarioRepositorio.ObtenerPorId(id);
        }

        public Resultado Insertar(Usuario usuario)
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

            // Hashear la contraseña antes de guardar en la DB
            usuario.Contrasena = _encriptador.Encriptar(usuario.Contrasena);
            _usuarioRepositorio.Insertar(usuario);

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

        public string GenerarNombreUsuario(string nombre, string apellido)
        {
            string baseUsername = $"{nombre}.{apellido}".ToLower().Replace(" ", "");
            string username = baseUsername;
            int counter = 1;

            while (_usuarioRepositorio.ExisteUsername(username))
            {
                username = baseUsername + counter;
                counter++;
            }

            return username;
        }

        public string GenerarContrasena(int longitud)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, longitud)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
            }
            else
            {
                loginResultado.Exito = false;
                loginResultado.Mensaje = "Contraseña incorrecta.";
                loginResultado.Token = null;
            }

            return loginResultado;
        }
    }
}
