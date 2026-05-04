using Microsoft.AspNetCore.Mvc;

namespace Servicio_Ventas.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
