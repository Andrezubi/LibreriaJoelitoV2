using FrontendLibreria.Adapters.Servicio2Adapters;
using FrontendLibreria.DTOs.Servicio2DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendLibreria.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioIndexModel : PageModel
    {
        private readonly IUsuarioServicioAdapter _usuarioAdapter;

        public UsuarioIndexModel(IUsuarioServicioAdapter usuarioAdapter)
        {
            _usuarioAdapter = usuarioAdapter;
        }

        public List<UsuarioDto> Usuarios { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }
        
        [TempData]
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public SolicitudCrearUsuarioDto UsuarioEditar { get; set; } = new();

        [BindProperty]
        public int IdEditar { get; set; }

        public List<string> ErroresValidacion { get; set; } = new();

        public async Task OnGetAsync()
        {
            Usuarios = await _usuarioAdapter.ObtenerTodos();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var exito = await _usuarioAdapter.Eliminar(id);
            if (exito)
            {
                SuccessMessage = "Usuario eliminado (baja lógica) exitosamente.";
            }
            else
            {
                ErrorMessage = "No se pudo eliminar el usuario.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            var resultado = await _usuarioAdapter.Actualizar(IdEditar, UsuarioEditar);
            if (resultado.Exito)
            {
                SuccessMessage = "Usuario actualizado exitosamente.";
                return RedirectToPage();
            }

            ErroresValidacion = resultado.Errores;
            Usuarios = await _usuarioAdapter.ObtenerTodos();
            return Page();
        }

        public async Task<IActionResult> OnGetObtenerUsuarioAsync(int id)
        {
            var usuario = await _usuarioAdapter.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return new JsonResult(usuario);
        }
    }
}
