using LibreriaJoelito.Aplicacion.Interfaces;
using LibreriaJoelito.Aplicacion.Servicios;
using LibreriaJoelito.Dominio.Models;
using LibreriaJoelito.Dominio.Validators;
using LibreriaJoelito.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

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
        private readonly ProductoServicio productoServicio;
        private readonly PresentacionServicio _presentacionService;

        public ProductoCreateModel(
            IConfiguration configuration,
            ProductoServicio productoServicio,
            PresentacionServicio presentacionService)
        {
            this.configuration = configuration;
            this.productoServicio = productoServicio;
            this._presentacionService = presentacionService;
        }

        public void OnGet()
        {
            CargarListas();
        }

        public IActionResult OnPost()
        {
            producto.IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

            // Llamamos al servicio con la lógica atómica
            var result = productoServicio.Insert(producto, IdPresentacionSeleccionada, FactorConversion, PrecioVenta);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
                CargarListas();
                return Page();
            }

            MensajeExito = "El producto y su presentación inicial fueron creados correctamente.";
            return RedirectToPage("MostrarProductos");
        }

        private void CargarListas()
        {
            CategoriasDataTable = LoadCategorias();
            MarcasDataTable = LoadMarcas();
            // ¡Uso correcto del servicio en el PageModel!
            PresentacionesDataTable = _presentacionService.GetAll();
        }

        // --- LÓGICA HARDCODEADA DE CATEGORÍA Y MARCA (Como lo solicitaste) ---
        DataTable LoadCategorias()
        {
            string query = @"SELECT Id, Nombre FROM categoria WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);
            return bd.ExecuteReturningDataTable(cmd);
        }

        DataTable LoadMarcas()
        {
            string query = @"SELECT Id, Nombre FROM marca WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);
            return bd.ExecuteReturningDataTable(cmd);
        }

        public class NombreSimple { public string Nombre { get; set; } }

        [ValidateAntiForgeryToken]
        public JsonResult OnPostCrearCategoria([FromBody] NombreSimple data)
        {
            data.Nombre = data.Nombre?.Trim();
            if (string.IsNullOrWhiteSpace(data.Nombre)) return new JsonResult(new { ok = false, mensaje = "Nombre vacio" });

            try
            {
                var errors = ExtraValidator.ValidarNombreCategoria(data.Nombre);
                if (errors.Any()) return new JsonResult(new { success = false, message = errors.First().ErrorMessage });

                string query = "INSERT INTO categoria (Nombre, IdUsuario) VALUES (@nombre, @idUsuario);";
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Parameters.AddWithValue("@nombre", data.Nombre);
                cmd.Parameters.AddWithValue("@idUsuario", int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1"));
                bd.ExecuteNonQuery(cmd);
                LoadCategorias();
                return new JsonResult(new { ok = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, mensaje = ex.Message });
            }
        }

        [ValidateAntiForgeryToken]
        public JsonResult OnPostCrearMarca([FromBody] NombreSimple data)
        {
            if (string.IsNullOrWhiteSpace(data.Nombre)) return new JsonResult(new { ok = false, mensaje = "Nombre vacio" });

            try
            {
                var errores = ExtraValidator.ValidarNombreMarca(data.Nombre);
                if (errores.Any()) return new JsonResult(new { success = false, message = errores.First().ErrorMessage });

                string query = "INSERT INTO marca (Nombre) VALUES (@nombre);";
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Parameters.AddWithValue("@nombre", data.Nombre);
                bd.ExecuteNonQuery(cmd);
                LoadMarcas();
                return new JsonResult(new { ok = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, mensaje = ex.Message });
            }
        }
    }
}