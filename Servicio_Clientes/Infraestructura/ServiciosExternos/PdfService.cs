using Servicio_Clientes.Aplicacion.Interfaces;
using Servicio_Clientes.Dominio.Models;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.IO;

namespace Servicio_Clientes.Infraestructura.ServiciosExternos
{
    public class PdfService : IPdfService
    {

        private readonly IWebHostEnvironment _env;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerarComprobanteVenta<T>(List<T> datosVenta) 
        {
            if (datosVenta == null || datosVenta.Count == 0) return Array.Empty<byte>();

            var cabecera = datosVenta;
            decimal total = Convert.ToDecimal(cabecera.Total);

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    // --- CABECERA ---
                    page.Header().Column(col => {
                        col.Item().Row(row => {
                            string logoPath = Path.Combine(_env.WebRootPath, "img", "logo-lib.png");
                            row.ConstantItem(80).Height(80).Border(1).AlignCenter().AlignMiddle().Image(logoPath);
                            row.RelativeItem().PaddingLeft(20).AlignMiddle().Text("COMPROBANTE DE VENTA").FontSize(24).Bold();
                        });
                        // Acceso directo a propiedades del objeto
                        col.Item().PaddingTop(20).Text($"Fecha: {Convert.ToDateTime(cabecera.Fecha):dd/MM/yyyy}").Bold();
                        col.Item().Text($"CI/NIT: {cabecera.Ci}").Bold();
                        col.Item().Text($"Razón Social: {cabecera.ClienteNombre} {cabecera.ApellidoPaterno}".Trim()).Bold();
                    });

                    page.Content().PaddingVertical(20).Column(col => {
                        col.Item().Table(tabla => {
                            tabla.ColumnsDefinition(c => {
                                c.ConstantColumn(60); c.RelativeColumn(); c.ConstantColumn(80); c.ConstantColumn(80);
                            });
                            tabla.Header(h => {
                                h.Cell().Border(1).Padding(5).Text("Cant.");
                                h.Cell().Border(1).Padding(5).Text("Descripción");
                                h.Cell().Border(1).Padding(5).Text("P. Unit Bs.");
                                h.Cell().Border(1).Padding(5).Text("Importe BS.");
                            });

                            // Iteramos sobre la lista de objetos T
                            foreach (var fila in datosVenta)
                            {
                                tabla.Cell().Border(1).Padding(5).Text(fila.Cantidad.ToString());
                                tabla.Cell().Border(1).Padding(5).Text(fila.DescripcionProducto.ToString());
                                tabla.Cell().Border(1).Padding(5).Text(Convert.ToDecimal(fila.PrecioUnitario).ToString("N2"));
                                tabla.Cell().Border(1).Padding(5).Text(Convert.ToDecimal(fila.Subtotal).ToString("N2"));
                            }
                        });
                        col.Item().AlignRight().PaddingTop(10).Text($"Total Bs: {total:N2}").Bold();
                        col.Item().Text($"Son: {NumeroALetras(total)}").Bold();
                    });

                    string nombreEmpleado = cabecera.NombreEmpleado.ToString();

                    page.Footer().AlignRight().Text($"{DateTime.Now:dd/MM/yyyy HH:mm} - {nombreEmpleado}").Italic();
                });
            });

            return documento.GeneratePdf();
        }


        private string NumeroALetras(decimal numero)
        {
            long entero = (long)Math.Truncate(numero);
            int centavos = (int)Math.Round((numero - entero) * 100);

            string letras = entero == 0 ? "CERO" : ConvertirEnteroALetras(entero);

            letras = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(letras.ToLower());

            return $"{letras} {centavos:00}/100";
        }


        //TODO cambiar por una librería externa para convertir números a letras, o extraer a una clase de constantes aparte , o al menos optimizar este método que es muy largo y repetitivo
        private string ConvertirEnteroALetras(long numero)
        {
            if (numero == 0) return "";
            if (numero == 1) return "UN";
            if (numero == 2) return "DOS";
            if (numero == 3) return "TRES";
            if (numero == 4) return "CUATRO";
            if (numero == 5) return "CINCO";
            if (numero == 6) return "SEIS";
            if (numero == 7) return "SIETE";
            if (numero == 8) return "OCHO";
            if (numero == 9) return "NUEVE";
            if (numero == 10) return "DIEZ";
            if (numero == 11) return "ONCE";
            if (numero == 12) return "DOCE";
            if (numero == 13) return "TRECE";
            if (numero == 14) return "CATORCE";
            if (numero == 15) return "QUINCE";
            if (numero < 20) return "DIECI" + ConvertirEnteroALetras(numero - 10);
            if (numero == 20) return "VEINTE";
            if (numero < 30) return "VEINTI" + ConvertirEnteroALetras(numero - 20);
            if (numero == 30) return "TREINTA";
            if (numero == 40) return "CUARENTA";
            if (numero == 50) return "CINCUENTA";
            if (numero == 60) return "SESENTA";
            if (numero == 70) return "SETENTA";
            if (numero == 80) return "OCHENTA";
            if (numero == 90) return "NOVENTA";

            if (numero < 100) return ConvertirEnteroALetras((numero / 10) * 10) + " Y " + ConvertirEnteroALetras(numero % 10);

            if (numero == 100) return "CIEN";
            if (numero < 200) return "CIENTO " + ConvertirEnteroALetras(numero - 100);
            if (numero == 200) return "DOSCIENTOS";
            if (numero == 300) return "TRESCIENTOS";
            if (numero == 400) return "CUATROCIENTOS";
            if (numero == 500) return "QUINIENTOS";
            if (numero == 600) return "SEISCIENTOS";
            if (numero == 700) return "SETECIENTOS";
            if (numero == 800) return "OCHOCIENTOS";
            if (numero == 900) return "NOVECIENTOS";

            if (numero < 1000) return ConvertirEnteroALetras((numero / 100) * 100) + " " + ConvertirEnteroALetras(numero % 100);

            if (numero == 1000) return "MIL";
            if (numero < 2000) return "MIL " + ConvertirEnteroALetras(numero % 1000);

            if (numero < 1000000)
            {
                string miles = ConvertirEnteroALetras(numero / 1000) + " MIL";
                string resto = ConvertirEnteroALetras(numero % 1000);
                return resto == "" ? miles : miles + " " + resto;
            }

            if (numero == 1000000) return "UN MILLON";
            if (numero < 2000000) return "UN MILLON " + ConvertirEnteroALetras(numero % 1000000);

            if (numero < 1000000000000)
            {
                string millones = ConvertirEnteroALetras(numero / 1000000) + " MILLONES";
                string resto = ConvertirEnteroALetras(numero % 1000000);
                return resto == "" ? millones : millones + " " + resto;
            }

            return "";
        }
    }
}