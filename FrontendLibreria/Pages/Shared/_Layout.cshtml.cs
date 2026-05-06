using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Shared
{
    public class _LayoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogout()
        {
            System.Console.WriteLine("Antes:"+User.ToString());
            await HttpContext.SignOutAsync(); 
            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                Path = "/" 
            });
            await HttpContext.SignOutAsync();
            System.Console.WriteLine("Antes:" + User.ToString());

            return RedirectToPage("/LogIn/InicioSesion");
        }
    }
}
