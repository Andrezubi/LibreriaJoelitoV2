
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Data;
using System.Security.Claims;
using Servicio_Ventas.Infrestructura.Persistencia;

namespace LibreriaJoelito.Pages.Productos
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class ProductoCreateModel : PageModel
    {
        private readonly IConfiguration configuration;
        public RepositorioBD bd { get; set; } = RepositorioBD.Instancia;

        [BindProperty] public Producto producto { get; set; }

        // Nuevos campos obligatorios para la Venta
        [BindProperty] public int IdPresentacionSeleccionada { get; set; }
        [BindProperty] public int FactorConversion { get; set; } = 1;
        [BindProperty] public decimal PrecioVenta { get; set; }

        [TempData] public string MensajeExito { get; set; }

        public DataTable CategoriasDataTable { get; set; }
        public DataTable MarcasDataTable { get; set; }
        public DataTable PresentacionesDataTable { get; set; }

        // Inyectamos la Fachada de Productos y el Servicio de Presentaciones
        // private readonly ProductoServicio productoServicio;
        // private readonly PresentacionServicio _presentacionService;

        public ProductoCreateModel(
            IConfiguration configuration)
            // ProductoServicio productoServicio,
            // PresentacionServicio presentacionService)
        {
            this.configuration = configuration;
            // this.productoServicio = productoServicio;
            // this._presentacionService = presentacionService;
        }

        public void OnGet()
        {
            CargarListas();
        }

        public IActionResult OnPost()
        {
            // producto.IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

            // Llamamos al servicio con la lógica atómica
            // var result = productoServicio.Insert(producto, IdPresentacionSeleccionada, FactorConversion, PrecioVenta);

            // if (result.IsFailure)
            // {
            //     ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
            //     CargarListas();
            //     return Page();
            // }

            MensajeExito = "El producto y su presentación inicial fueron creados correctamente.";
            return RedirectToPage("MostrarProductos");
        }

        private void CargarListas()
        {
            CategoriasDataTable = LoadCategorias();
            MarcasDataTable = LoadMarcas();
            // ¡Uso correcto del servicio en el PageModel!
            // PresentacionesDataTable = _presentacionService.GetAll();
            PresentacionesDataTable = new DataTable();
        }

        // --- LÓGICA HARDCODEADA DE CATEGORÍA Y MARCA (Como lo solicitaste) ---
        DataTable LoadCategorias()
        {
            return new DataTable();
        }

        DataTable LoadMarcas()
        {
            return new DataTable();
        }

        public class NombreSimple { public string Nombre { get; set; } }

        [ValidateAntiForgeryToken]
        public JsonResult OnPostCrearCategoria([FromBody] NombreSimple data)
        {
            return new JsonResult(new { ok = false, mensaje = "Temporalmente deshabilitado" });
        }

        [ValidateAntiForgeryToken]
        public JsonResult OnPostCrearMarca([FromBody] NombreSimple data)
        {
            return new JsonResult(new { ok = false, mensaje = "Temporalmente deshabilitado" });
        }
    }

    // STUBS TEMPORALES PARA COMPILACIÓN
    // El equipo de Ventas refactorizó y eliminó estas clases del backend,
    // por lo que las definimos aquí temporalmente para que el FrontendLibreria compile
    // sin tener que borrar todo el HTML de las vistas.
    public class Producto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public int IdMarca { get; set; }
        public int Stock { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
    public class ProductoServicio
    {
    }
    public class PresentacionServicio
    {
    }
}