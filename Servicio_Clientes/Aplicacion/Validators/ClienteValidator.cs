using LibreriaJoelitoV2.Dominio.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LibreriaJoelitoV2.Dominio.Validators
{
    public class ClienteValidator
    {
        public string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            texto = Regex.Replace(texto.Trim(), @"\s+", " ");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
        }

        public List<ValidationResult> Validar(Cliente cliente)
        {
            var errores = new List<ValidationResult>();

            ValidarNombre(cliente.Nombre, errores);
            ValidarApellidoPaterno(cliente.ApellidoPaterno, errores);
            ValidarApellidoMaterno(cliente.ApellidoMaterno, errores);
            ValidarCI(cliente.Ci, errores);
            ValidarComplemento(cliente.Complemento, errores);
            ValidarEmail(cliente.Email, errores);

            return errores;
        }

        void ValidarNombre(string nombre, List<ValidationResult> errores)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                errores.Add(new ValidationResult("El Nombre es obligatorio.", new[] { "_cliente.Nombre" }));
                return;
            }
            if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errores.Add(new ValidationResult("El Nombre solo puede contener letras y espacios.", new[] { "_cliente.Nombre" }));
            }
        }

        void ValidarApellidoPaterno(string apellido, List<ValidationResult> errores)
        {
            if (string.IsNullOrWhiteSpace(apellido))
            {
                errores.Add(new ValidationResult("El Apellido Paterno es obligatorio.", new[] { "_cliente.ApellidoPaterno" }));
                return;
            }
            if (!Regex.IsMatch(apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errores.Add(new ValidationResult("El Apellido Paterno solo puede contener letras y espacios.", new[] { "_cliente.ApellidoPaterno" }));
            }
        }

        void ValidarApellidoMaterno(string? apellido, List<ValidationResult> errores)
        {
            if (!string.IsNullOrWhiteSpace(apellido))
            {
                if (!Regex.IsMatch(apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    errores.Add(new ValidationResult("El Apellido Materno solo puede contener letras y espacios.", new[] { "_cliente.ApellidoMaterno" }));
                }
            }
        }

        void ValidarCI(string ci, List<ValidationResult> errores)
        {
            if (string.IsNullOrWhiteSpace(ci))
            {
                errores.Add(new ValidationResult("El CI es obligatorio.", new[] { "_cliente.Ci" }));
            }
            else
            {
                if (ci.Length < 6 || ci.Length > 10)
                    errores.Add(new ValidationResult("El CI debe tener entre 6 y 10 caracteres.", new[] { "_cliente.Ci" }));
                if (!Regex.IsMatch(ci, @"^\d+$"))
                    errores.Add(new ValidationResult("El CI solo puede contener números.", new[] { "_cliente.Ci" }));
            }
        }

        void ValidarEmail(string? email, List<ValidationResult> errores)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errores.Add(new ValidationResult("El Email no tiene un formato válido.", new[] { "_cliente.Email" }));
                }
            }
        }

        void ValidarComplemento(string? complemento, List<ValidationResult> errores)
        {
            if (!string.IsNullOrWhiteSpace(complemento))
            {
                if (!Regex.IsMatch(complemento, @"^[0-9][A-Z]$"))
                {
                    errores.Add(new ValidationResult("El Complemento debe tener un formato de número seguido de letra mayúscula (ej: 1A).", new[] { "_cliente.Complemento" }));
                }
            }
        }
    }
}
