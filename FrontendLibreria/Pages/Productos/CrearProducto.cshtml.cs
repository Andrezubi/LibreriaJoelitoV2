
using FrontendLibreria.Adapters.Producto;
using FrontendLibreria.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Data;
using System.Security.Claims;

// Pages/Productos/CrearProducto.cshtml.cs
namespace FrontendLibreria.Pages.Productos
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class CrearProductoModel : PageModel
    {
        private readonly IAdaptadorProducto _productoAdapter;


        public CrearProductoModel(
            IAdaptadorProducto productoAdapter)

        {
            _productoAdapter = productoAdapter;

        }

        // Campos del formulario principal
        [BindProperty] public string Nombre { get; set; } = "";
        [BindProperty] public int IdCategoria { get; set; }
        [BindProperty] public int IdMarca { get; set; }
        [BindProperty] public int Stock { get; set; }
        [BindProperty] public int IdPresentacionSeleccionada { get; set; }
        [BindProperty] public int FactorConversion { get; set; } = 1;
        [BindProperty] public decimal PrecioVenta { get; set; }

        // Listas para los selects
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<MarcaDto> Marcas { get; set; } = new();
        public List<PresentacionDto> Presentaciones { get; set; } = new();

        [TempData] public string? MensajeExito { get; set; }

        public async Task OnGetAsync()
        {
            await CargarListasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int idUsuario = ObtenerIdUsuario();

            var result = await _productoAdapter.CrearProductoAsync(new ProductoDto
            {
                Nombre = Nombre,
                IdCategoria = IdCategoria,
                IdMarca = IdMarca,
                Stock = Stock,
                IdUsuario = idUsuario
            },IdPresentacionSeleccionada,FactorConversion,PrecioVenta);


            var result2 = await _productoAdapter.AgregarPresentacionAsync(new SolicitudAgregarPresentacion
            {
                IdPresentacion = IdPresentacionSeleccionada,
                FactorConversion = FactorConversion,
                PrecioVenta = PrecioVenta,
                IdUsuario = idUsuario

            });

            if (!result.Success || !result2.Success)
            {
                var errores = result.Errors.Concat(result2.Errors);
                ModelState.AddModelError(string.Empty, string.Join(", ", errores));

                await CargarListasAsync();
                return Page();
            }

            MensajeExito = "El producto y su presentación inicial fueron creados correctamente.";
            return RedirectToPage("IndiceProductos");
        }

        // Handler — crear categoría rápida desde el modal
        public async Task<JsonResult> OnPostCrearCategoriaAsync([FromBody] NombreRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Nombre))
                return new JsonResult(new { ok = false, mensaje = "El nombre es obligatorio" });

            var result = await _productoAdapter.CrearCategoriaAsync(data.Nombre.Trim(), ObtenerIdUsuario());

            if (!result.Success)
                return new JsonResult(new { ok = false, mensaje = result.Errors.FirstOrDefault() });

            return new JsonResult(new { ok = true });
        }

        

        // Helpers privados
        private async Task CargarListasAsync()
        {
            var categoriasTask = _productoAdapter.GetCategoriasAsync();
            var marcasTask = _productoAdapter.GetMarcasAsync();
            var presentacionesTask = _productoAdapter.GetPresentacionesAsync();

            await Task.WhenAll(categoriasTask, marcasTask, presentacionesTask);

            Categorias = categoriasTask.Result;
            Marcas = marcasTask.Result;
            Presentaciones = presentacionesTask.Result;
        }

        private int ObtenerIdUsuario()
            => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
    }

    // DTO local para los modales de creación rápida
    public class NombreRequest
    {
        public string Nombre { get; set; } = "";
    }
}