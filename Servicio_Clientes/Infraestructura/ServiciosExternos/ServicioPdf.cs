using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Servicio_Clientes.Aplicacion.Interfaces;
using System.Data;

namespace Servicio_Clientes.Infraestructura.ServiciosExternos
{
    public class ServicioPdf : IServicioPdf
    {
        public byte[] GenerarReporteUsuariosPdf(DataTable usuarios)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    page.Header().Text("Reporte de Usuarios - Librería Joelito")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ID").SemiBold();
                            header.Cell().Text("Nombre").SemiBold();
                            header.Cell().Text("Correo").SemiBold();
                            header.Cell().Text("Rol").SemiBold();
                        });

                        foreach (DataRow row in usuarios.Rows)
                        {
                            table.Cell().Text(row["Id"].ToString());
                            table.Cell().Text($"{row["Nombre"]} {row["ApellidoPaterno"]}");
                            table.Cell().Text(row["Email"].ToString());
                            table.Cell().Text(row["Rol"].ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }
    }
}