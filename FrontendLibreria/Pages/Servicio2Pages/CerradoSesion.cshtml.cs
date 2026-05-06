using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendLibreria.Pages.Servicio2Pages
{
    public class CerradoSesionModel : PageModel
    {
        public void OnGet()
        {
            
        }
        public async Task<IActionResult> OnPost()
        {
            // Comando oficial para cerrar sesión y borrar la cookie de seguridad
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirigir a la pantalla de login
            return RedirectToPage("/Servicio2Pages/InicioSesion");
        }
    }
}
