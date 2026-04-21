using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaFacturacion.Application.DTOs;

namespace SistemaFacturacion.Application.Services
{
    public class PdfFacturaService
    {
        public byte[] GenerarPdf(FacturaResponseDto factura)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("FACTURA")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"Número de factura: {factura.NumeroFactura}");
                        column.Item().Text($"Fecha: {factura.Fecha:dd/MM/yyyy HH:mm}");
                        column.Item().Text($"Cliente: {factura.Cliente}");

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Producto").Bold();
                                header.Cell().Element(CellStyle).Text("Cantidad").Bold();
                                header.Cell().Element(CellStyle).Text("Precio").Bold();
                                header.Cell().Element(CellStyle).Text("Total").Bold();
                            });

                            foreach (var detalle in factura.Detalles)
                            {
                                table.Cell().Element(CellStyle).Text(detalle.Producto);
                                table.Cell().Element(CellStyle).Text(detalle.Cantidad.ToString());
                                table.Cell().Element(CellStyle).Text($"{detalle.PrecioUnitario:F2}");
                                table.Cell().Element(CellStyle).Text($"{detalle.TotalLinea:F2}");
                            }
                        });

                        column.Item().AlignRight().PaddingTop(10).Column(resumen =>
                        {
                            resumen.Item().Text($"Subtotal: ${factura.Subtotal:F2}");
                            resumen.Item().Text($"IVA (15%): ${factura.Iva:F2}");
                            resumen.Item().Text($"TOTAL: ${factura.Total:F2}")
                                .Bold()
                                .FontSize(14);
                        });
                    });
                });
            }).GeneratePdf();

            return pdf;
        }

        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5);
        }
    }
}