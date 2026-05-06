using FrontendLibreria.Adapters.Cliente;
using FrontendLibreria.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendLibreria.Pages.Clientes
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class CrearClienteModel : PageModel
    {
        private readonly IAdaptadorCliente _clienteAdapter;

        public CrearClienteModel(IAdaptadorCliente clienteAdapter)
        {
            _clienteAdapter = clienteAdapter;
        }

        [BindProperty] public string RazonSocial { get; set; } = "";
        [BindProperty] public string Ci { get; set; } = "";
        [BindProperty] public string? Complemento { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public bool ClienteFrecuente { get; set; }

        [TempData] public string? MensajeExito { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            int idUsuario = ObtenerIdUsuario();

            var result = await _clienteAdapter.InsertarAsync(new ClienteDto
            {
                RazonSocial = Normalizar(RazonSocial)!,
                Ci = Ci,
                Complemento = Complemento,
                Email = Email,
                ClienteFrecuente = ClienteFrecuente,
                IdUsuario = idUsuario
            });

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                return Page();
            }

            MensajeExito = $"Cliente '{RazonSocial}' creado exitosamente.";
            return RedirectToPage("ClientesGet");
        }

        private int ObtenerIdUsuario()
            => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

        private static string? Normalizar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return texto;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(texto.Trim().ToLower());
        }
    }
}