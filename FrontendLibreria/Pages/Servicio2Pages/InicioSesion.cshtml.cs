using FrontendLibreria.Adapters.Servicio2Adapters;
using FrontendLibreria.DTOs.Servicio2DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Servicio2Pages
{
    public class InicioSesionModel : PageModel
    {
        private readonly IUsuarioServicioAdapter _usuarioAdapter;

        public InicioSesionModel(IUsuarioServicioAdapter usuarioAdapter)
        {
            _usuarioAdapter = usuarioAdapter;
        }
        [BindProperty]
        public string NombreUsuario { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                ErrorMessage = "Debe ingresar usuario y contraseña.";
                return Page();
            }

            // usuario eliminado (estado = 0) no puede iniciar sesión   
            if (await _usuarioAdapter.EsUsuarioEliminado(NombreUsuario))
            {
                ErrorMessage = "El usuario ha sido eliminado. Contacte al administrador.";
                return Page();
            }

            var result = await _usuarioAdapter.Login(new SolicitudLoginDto
            {
                NombreUsuario = NombreUsuario,
                Contrasena = contrasena
            });

            if (!result.Exito)
            {
                ErrorMessage = result.Mensaje;
                return Page();
            }

            // Crear claims para la cookie de sesión
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.NombreUsuario),
                new Claim(ClaimTypes.Role, result.Rol),
                new Claim("Token", result.Token),
                new Claim("IdUsuario", result.IdUsuario.ToString()),
                new Claim("MustChangePassword", result.DebeCambiarContrasena.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false }
            );

            // Forzar cambio de contraseña si es primer login
            if (result.DebeCambiarContrasena)
                return RedirectToPage("/Usuarios/CambiarContrasena");

            return RedirectToPage("/Index");
        }

    }
}
