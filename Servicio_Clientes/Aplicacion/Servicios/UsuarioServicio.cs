using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Aplicacion.Results;
using Servicio_Clientes.Aplicacion.Validators;
using Servicio_Clientes.Dominio.Interfaces;
using Servicio_Clientes.Dominio.Models;
using System.Data;

namespace Servicio_Clientes.Aplicacion.Servicios
{
    public class UsuarioServicio
    {
        private readonly IRepository<Usuario> usuarioRepository;
        private readonly IUsuarioRepository extraRepo;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenService tokenService;

        public UsuarioServicio(IRepository<Usuario> usuarioRepository, IUsuarioRepository extraRepo, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            this.usuarioRepository = usuarioRepository;
            this.extraRepo = extraRepo;
            this.passwordHasher = passwordHasher;
            this.tokenService = tokenService;
        }

        public List<Usuario> GetAll()
        {
            return usuarioRepository.GetAll();
        }

        //public List<T> GetById(int id)
        //{
        //    return usuarioRepository.GetById(id);
        //}

        public Result Insert(Usuario usuario)
        {
            var validationResults = EmpleadoValidator.Validar(usuario);

            if (validationResults.Any())
            {
                var errors = validationResults
                    .Select(v =>
                    {
                        var field = v.MemberNames.FirstOrDefault() ?? "General";
                        return $"{field}: {v.ErrorMessage}";
                    })
                    .ToList();

                return Result.Failure(errors);
            }

            if (usuarioRepository.ExisteDuplicado(usuario))
            {
                return Result.Failure("empleado.Ci: El empleado con ese CI ya existe.");
            }

            // Hashear la contraseña antes de guardar en la DB
            usuario.Password = passwordHasher.Hash(usuario.Password);
            usuarioRepository.Insert(usuario);

            return Result.Success();
        }

        public Result Update(Usuario usuario)
        {
            var validationResults = EmpleadoValidator.Validar(usuario);

            if (validationResults.Any())
            {
                var errors = validationResults
                    .Select(v =>
                    {
                        return $"{v.ErrorMessage}";
                    })
                    .ToList();

                return Result.Failure(errors);
            }

            // Permitir actualización aunque el CI exista si es el mismo usuario, 
            // pero para esta lógica podemos depender de la base de datos o ajustar si es necesario.
            // Para ser seguros, lo guardamos directo.
            usuarioRepository.Update(usuario);

            return Result.Success();
        }

        public int Delete(Usuario usuario)
        {
            return usuarioRepository.Delete(usuario);
        }

        public string GenerarUsername(string nombre, string apellido)
        {
            string baseUsername = $"{nombre}.{apellido}".ToLower().Replace(" ", "");
            string username = baseUsername;
            int counter = 1;

            while (extraRepo.ExisteUsername(username))
            {
                username = baseUsername + counter;
                counter++;
            }

            return username;
        }

        public string GenerarPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Result<string> Login(string username, string password)
        {
            var user = extraRepo.GetDatosLogin(username);

            if (user == null)
            {
                return Result<string>.Failure("Usuario no encontrado.");
            }
            else if (passwordHasher.Verify(password, user.Password))
            {
                var token = tokenService.GenerarToken(username, user.Rol, user.Id.ToString());
                return Result<string>.Success(token);
            }
            else
            {
                return Result<string>.Failure("Contraseña incorrecta.");
            }
        }
    }
}
