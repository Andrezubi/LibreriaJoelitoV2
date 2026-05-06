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
            Console.WriteLine("ANTES LOGOUT: " + User.Identity?.Name);

            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                Path = "/"
            });

            Console.WriteLine("DESPUÉS LOGOUT: " + User.Identity?.Name);

            return RedirectToPage("/Index");
        }
    }
}
