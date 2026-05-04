using LibreriaJoelito.Aplicacion.Interfaces;
using LibreriaJoelito.Aplicacion.Servicios;
using LibreriaJoelito.Dominio.Models;
using LibreriaJoelito.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;

namespace LibreriaJoelito.Pages.Productos
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class MostrarProductosModel : PageModel
    {
        public RepositorioBD bd { get; set; } = RepositorioBD.Instancia;
        private readonly IConfiguration configuration;
        private readonly ProductoServicio productoServicio;
        private readonly PresentacionServicio _presentacionService; // Agregado

        public MostrarProductosModel(
            IConfiguration configuration,
            ProductoServicio productoServicio,
            PresentacionServicio presentacionService) // Inyectado
        {
            this.configuration = configuration;
            this.productoServicio = productoServicio;
            this._presentacionService = presentacionService;
        }

        public DataTable ProductosDataTable { get; set; } = new DataTable();
        public DataTable CategoriasDataTable { get; set; }
        public DataTable MarcasDataTable { get; set; }
        public DataTable PresentacionesDataTable { get; set; } // Agregado para el modal

        [BindProperty] public Producto producto { get; set; }

        [BindProperty] public int IdProductoSeleccionado { get; set; }
        [BindProperty] public int IdPresentacionSeleccionada { get; set; }
        [BindProperty] public int FactorConversion { get; set; } = 1;
        [BindProperty] public decimal PrecioVenta { get; set; }

        [TempData] public string MensajeExito { get; set; }

        public void OnGet()
        {
            LoadProductos();
            LoadCategorias();
            LoadMarcas();
            PresentacionesDataTable = _presentacionService.GetAll(); 
        }

        void LoadProductos()
        {
            ProductosDataTable = productoServicio.GetAll();
        }

        void LoadCategorias()
        {
            string query = @"SELECT Id, Nombre FROM categoria WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);
            CategoriasDataTable = bd.ExecuteReturningDataTable(cmd);
        }

        void LoadMarcas()
        {
            string query = @"SELECT Id, Nombre FROM marca WHERE estado = 1 ORDER BY Nombre";
            MySqlCommand cmd = new MySqlCommand(query);
            MarcasDataTable = bd.ExecuteReturningDataTable(cmd);
        }

        // ---> EL POST PARA AGREGAR LA PRESENTACIÓN <---
        public IActionResult OnPostAgregarPresentacion()
        {
            int idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

            var result = productoServicio.AsociarNuevaPresentacion(
                IdProductoSeleccionado,
                IdPresentacionSeleccionada,
                FactorConversion,
                PrecioVenta,
                idUsuario);

            if (result.IsFailure)
            {
                // Mostramos el error en la misma pantalla usando TempData (o ModelState)
                TempData["MensajeError"] = string.Join(", ", result.Errors);
                return RedirectToPage();
            }

            TempData["MensajeExito"] = "Presentación agregada exitosamente al producto.";
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            producto.IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
            productoServicio.Delete(producto);
            TempData["MensajeExito"] = "El producto fue eliminado correctamente.";
            return RedirectToPage("MostrarProductos");
        }

        public JsonResult OnPostUpdate()
        {
            try
            {
                producto.IdUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
                var result = productoServicio.Update(producto);

                if (result.IsFailure)
                    return new JsonResult(new { success = false, message = result.Errors });

                TempData["MensajeExito"] = "El producto fue editado correctamente.";
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public string getMarcaById(int? id)
        {
            if (id == null || id == 0) return "ERROR no tiene Marca";
            string query = "SELECT Nombre FROM marca WHERE Id=@id";
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);
            using (MySqlDataReader reader = bd.ExecuteReader(cmd))
            {
                if (reader.Read()) return reader["Nombre"].ToString()!;
            }
            return "No se encontro La Marca";
        }

        public string getCategoriaById(int? id)
        {
            if (id == null || id == 0) return "ERROR no tiene Categoria";
            string query = "SELECT Nombre FROM categoria WHERE Id=@id";
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);
            using (MySqlDataReader reader = bd.ExecuteReader(cmd))
            {
                if (reader.Read()) return reader["Nombre"].ToString()!;
            }
            return "No se encontro La Categoria";
        }
    }
}