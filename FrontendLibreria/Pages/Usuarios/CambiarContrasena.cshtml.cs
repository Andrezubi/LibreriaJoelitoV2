using FrontendLibreria.Adapters.Servicio2Adapters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Usuarios
{
    [Authorize]
    public class CambiarContrasenaModel : PageModel
    {
        private readonly IUsuarioServicioAdapter _adapter;

        public CambiarContrasenaModel(IUsuarioServicioAdapter adapter)
        {
            _adapter = adapter;
        }

        public List<string> Errores { get; set; } = new();
        public string MensajeExito { get; set; } = string.Empty;
        public bool EsPrimerLogin { get; set; }

        public void OnGet()
        {
            EsPrimerLogin = User.FindFirst("MustChangePassword")?.Value == "True";
        }

        public async Task<IActionResult> OnPostAsync(
            string contrasenaActual,
            string nuevaContrasena,
            string confirmacionContrasena)
        {
            EsPrimerLogin = User.FindFirst("MustChangePassword")?.Value == "True";

            var token = User.FindFirst("Token")?.Value ?? string.Empty;

            var (exito, errores) = await _adapter.CambiarContrasena(
                contrasenaActual, nuevaContrasena, confirmacionContrasena, token);

            if (!exito)
            {
                Errores = errores;
                return Page();
            }

            // Renovar cookie sin MustChangePassword
            var claims = User.Claims
                .Where(c => c.Type != "MustChangePassword")
                .Append(new Claim("MustChangePassword", "False"))
                .ToList();

            var identity = new System.Security.Claims.ClaimsIdentity(
                claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new System.Security.Claims.ClaimsPrincipal(identity));

            MensajeExito = "Contraseña actualizada correctamente.";

            return RedirectToPage("/Index");
        }
    }
}
