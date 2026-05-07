using ClosedXML.Excel;
using Servicio_Clientes.Dominio.Models.DTOs;
using Servicio_Clientes.Infraestructura.Persistencia.FactoryProducts;
using SkiaSharp;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Servicio_Clientes.Aplicacion.Servicios
{
    public class ReporteServicio
    {
        private readonly ReporteRepositorio _repo;
        private readonly IWebHostEnvironment _env;

        public ReporteServicio(ReporteRepositorio repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        // ── PDF ────────────────────────────────────────────────────────────
        public byte[] GenerarPdf(DateTime fechaDesde, DateTime fechaHasta, string usuario)
        {
            var datos = _repo.ObtenerVentasPorCategoria(fechaDesde, fechaHasta);
            var logoPath = Path.Combine(_env.ContentRootPath, "Recursos", "Imagenes", "logo-lib.png");
            byte[] logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : Array.Empty<byte>();
            byte[] graficoPng = GenerarGraficoTorta(datos);

            decimal totalUnidades = datos.Sum(d => d.TotalUnidades);
            decimal totalRecaudado = datos.Sum(d => d.TotalRecaudado);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    // ENCABEZADO
                    page.Header().Row(row =>
                    {
                        if (logoBytes.Length > 0)
                            row.ConstantItem(60).Image(logoBytes).FitArea();

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter()
                               .Text("LIBRERÍA JOELITO")
                               .FontSize(14).Bold()
                               .FontColor(Color.FromHex("#1a237e"));

                            col.Item().AlignCenter()
                               .Text("RECAUDACIÓN POR CATEGORÍA DE PRODUCTO")
                               .FontSize(11).Bold();

                            col.Item().AlignCenter()
                               .Text($"Desde: {fechaDesde:dd/MM/yyyy}  al  {fechaHasta:dd/MM/yyyy}")
                               .FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // CONTENIDO
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Tabla sumariada
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            // Encabezados
                            static IContainer CeldaHeader(IContainer c) =>
                                c.Background(Color.FromHex("#1a237e"))
                                 .Padding(6)
                                 .AlignCenter();

                            tabla.Header(h =>
                            {
                                h.Cell().Element(CeldaHeader)
                                 .Text("Categoría").FontColor(Colors.White).Bold();
                                h.Cell().Element(CeldaHeader)
                                 .Text("Total Unidades").FontColor(Colors.White).Bold();
                                h.Cell().Element(CeldaHeader)
                                 .Text("Total Recaudado Bs.").FontColor(Colors.White).Bold();
                            });

                            // Filas
                            bool par = false;
                            foreach (var item in datos)
                            {
                                var bg = par ? Color.FromHex("#e8eaf6") : Colors.White;
                                par = !par;

                                tabla.Cell().Background(bg).Padding(6)
                                     .Text(item.Categoria);
                                tabla.Cell().Background(bg).Padding(6).AlignCenter()
                                     .Text(item.TotalUnidades.ToString());
                                tabla.Cell().Background(bg).Padding(6).AlignRight()
                                     .Text(item.TotalRecaudado.ToString("N2"));
                            }

                            // Fila totales
                            tabla.Cell().Background(Color.FromHex("#c5cae9"))
                                 .Padding(6).Text("TOTAL").Bold();
                            tabla.Cell().Background(Color.FromHex("#c5cae9"))
                                 .Padding(6).AlignCenter()
                                 .Text(totalUnidades.ToString()).Bold();
                            tabla.Cell().Background(Color.FromHex("#c5cae9"))
                                 .Padding(6).AlignRight()
                                 .Text(totalRecaudado.ToString("N2")).Bold();
                        });

                        // Gráfico de torta
                        col.Item().PaddingTop(24).AlignCenter()
                           .Width(300).Height(300)
                           .Image(graficoPng).FitArea();
                    });

                    // PIE
                    page.Footer().AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span($"Reporte generado por: {usuario}  —  ");
                            txt.Span($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}  —  ");
                            txt.Span("Página ").FontSize(9);
                            txt.CurrentPageNumber().FontSize(9);
                            txt.Span(" de ").FontSize(9);
                            txt.TotalPages().FontSize(9);
                        });
                });
            });

            return doc.GeneratePdf();
        }

        // ── EXCEL ──────────────────────────────────────────────────────────
        public byte[] GenerarExcel(DateTime fechaDesde, DateTime fechaHasta, string usuario)
        {
            var datos = _repo.ObtenerVentasPorCategoria(fechaDesde, fechaHasta);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Ventas por Categoría");

            // Título
            ws.Cell(1, 1).Value = "LIBRERÍA JOELITO — RECAUDACIÓN POR CATEGORÍA";
            ws.Range(1, 1, 1, 3).Merge()
              .Style.Font.SetBold(true)
                    .Font.SetFontSize(13)
                    .Font.SetFontColor(XLColor.FromHtml("#1a237e"))
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(2, 1).Value = $"Desde: {fechaDesde:dd/MM/yyyy}  al  {fechaHasta:dd/MM/yyyy}";
            ws.Range(2, 1, 2, 3).Merge()
              .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Encabezados tabla
            int fila = 4;
            string[] headers = { "Categoría", "Total Unidades Vendidas", "Total Recaudado Bs." };
            for (int i = 0; i < headers.Length; i++)
            {
                var celda = ws.Cell(fila, i + 1);
                celda.Value = headers[i];
                celda.Style.Font.SetBold(true)
                           .Font.SetFontColor(XLColor.White)
                           .Fill.SetBackgroundColor(XLColor.FromHtml("#1a237e"))
                           .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                           .Border.SetBottomBorder(XLBorderStyleValues.Thin);
            }

            // Datos
            bool par = false;
            foreach (var item in datos)
            {
                fila++;
                var color = par ? XLColor.FromHtml("#e8eaf6") : XLColor.White;
                par = !par;

                ws.Cell(fila, 1).Value = item.Categoria;
                ws.Cell(fila, 2).Value = item.TotalUnidades;
                ws.Cell(fila, 2).Style.Alignment
                  .SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(fila, 3).Value = item.TotalRecaudado;
                ws.Cell(fila, 3).Style.NumberFormat.Format = "#,##0.00";

                ws.Range(fila, 1, fila, 3).Style
                  .Fill.SetBackgroundColor(color);
            }

            // Fila totales
            fila++;
            ws.Cell(fila, 1).Value = "TOTAL";
            ws.Cell(fila, 2).Value = datos.Sum(d => d.TotalUnidades);
            ws.Cell(fila, 3).Value = datos.Sum(d => d.TotalRecaudado);
            ws.Cell(fila, 3).Style.NumberFormat.Format = "#,##0.00";
            ws.Range(fila, 1, fila, 3).Style
              .Font.SetBold(true)
              .Fill.SetBackgroundColor(XLColor.FromHtml("#c5cae9"));

            // Pie
            fila += 2;
            ws.Cell(fila, 1).Value = $"Generado por: {usuario}  —  {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            ws.Range(fila, 1, fila, 3).Merge()
              .Style.Font.SetItalic(true)
                    .Font.SetFontColor(XLColor.Gray);

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        // ── GRÁFICO TORTA con SkiaSharp ────────────────────────────────────
        private byte[] GenerarGraficoTorta(List<ReporteVentaCategoriaDto> datos)
        {
            const int W = 600, H = 400;
            using var surface = SKSurface.Create(new SKImageInfo(W, H));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            if (!datos.Any())
            {
                using var snap = surface.Snapshot();
                return snap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
            }

            // Colores del gráfico
            var colores = new[]
            {
                SKColor.Parse("#1a237e"), SKColor.Parse("#3949ab"),
                SKColor.Parse("#7986cb"), SKColor.Parse("#c5cae9"),
                SKColor.Parse("#ff7043"), SKColor.Parse("#ffa726"),
                SKColor.Parse("#66bb6a"), SKColor.Parse("#26c6da")
            };

            decimal total = datos.Sum(d => d.TotalRecaudado);
            float startAngle = -90f;
            var rectTorta = new SKRect(60, 40, 360, 340);

            // Dibujar sectores
            for (int i = 0; i < datos.Count; i++)
            {
                float sweep = (float)(datos[i].TotalRecaudado / total * 360m);
                using var paint = new SKPaint
                {
                    Color = colores[i % colores.Length],
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawArc(rectTorta, startAngle, sweep, true, paint);

                // Borde blanco entre sectores
                using var border = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2
                };
                canvas.DrawArc(rectTorta, startAngle, sweep, true, border);
                startAngle += sweep;
            }

            // Leyenda
            float leyY = 50;
            for (int i = 0; i < datos.Count; i++)
            {
                float porcentaje = (float)(datos[i].TotalRecaudado / total * 100m);

                using var rectPaint = new SKPaint
                {
                    Color = colores[i % colores.Length],
                    IsAntialias = true
                };
                canvas.DrawRect(new SKRect(375, leyY, 395, leyY + 14), rectPaint);

                using var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true,
                    TextSize = 11
                };
                canvas.DrawText(
                    $"{datos[i].Categoria} ({porcentaje:F1}%)",
                    400, leyY + 12, textPaint);

                leyY += 24;
            }

            // Título del gráfico
            using var tituloPaint = new SKPaint
            {
                Color = SKColor.Parse("#1a237e"),
                IsAntialias = true,
                TextSize = 14,
                FakeBoldText = true
            };
            canvas.DrawText("Ventas por Categoría", 160, 380, tituloPaint);

            using var snapshot = surface.Snapshot();
            return snapshot.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }
    }
}
