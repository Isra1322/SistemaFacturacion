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

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo-fis.png");
            byte[]? logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;

            var rojoUTA = "#A30000";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Row(innerRow =>
                            {
                                innerRow.ConstantItem(90).Height(90).AlignMiddle().AlignCenter().Element(container =>
                                {
                                    if (logoBytes != null)
                                        container.Image(logoBytes, ImageScaling.FitArea);
                                    else
                                        container.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle()
                                            .Text("LOGO").FontColor(Colors.Grey.Darken1);
                                });

                                innerRow.RelativeItem().PaddingLeft(12).Column(col =>
                                {
                                    col.Spacing(4);

                                    col.Item().Text("UTA Tech")
                                        .FontSize(20)
                                        .Bold()
                                        .FontColor(rojoUTA);

                                    col.Item().Text("Sistema de Facturación")
                                        .FontSize(11)
                                        .FontColor(Colors.Grey.Darken1);

                                    col.Item().Text("Dirección: Av. Los Chasquis y Río Payamino, Huachi - Ambato")
                                        .FontSize(10);

                                    col.Item().Text("Teléfono: 0939579158")
                                        .FontSize(10);

                                    col.Item().Text("RUC: 9999999999001")
                                        .FontSize(10)
                                        .Bold();
                                });
                            });

                            row.ConstantItem(170).AlignRight().Column(col =>
                            {
                                col.Item().Background(rojoUTA)
                                    .PaddingVertical(12)
                                    .AlignCenter()
                                    .Text("FACTURA")
                                    .FontSize(20)
                                    .Bold()
                                    .FontColor(Colors.White);

                                col.Item().Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Column(info =>
                                    {
                                        info.Item().AlignCenter().Text($"N° {factura.NumeroFactura}")
                                            .Bold()
                                            .FontSize(13);

                                        info.Item().PaddingTop(6).AlignCenter().Text($"Fecha: {factura.Fecha:dd/MM/yyyy HH:mm}")
                                            .FontSize(10);
                                    });
                            });
                        });

                        header.Item().PaddingTop(14).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(18).Column(column =>
                    {
                        column.Spacing(18);

                        column.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(col =>
                        {
                            col.Spacing(6);
                            col.Item().Text("DATOS DEL CLIENTE")
                            .Bold()
                            .FontColor(rojoUTA)
                            .FontSize(12);

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(5).Text($"Cliente: {factura.Cliente}");
                            col.Item().Text($"Correo: {factura.Correo}");
                            col.Item().Text($"Teléfono: {factura.Telefono}");
                            col.Item().Text($"Dirección: {factura.Direccion}");
                        });

                        column.Item().Text("DETALLE DE PRODUCTOS")
                            .Bold()
                            .FontSize(13)
                            .FontColor(rojoUTA);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(5);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).Text("Producto").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Cantidad").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Precio Unit.").Bold().FontColor(Colors.White);
                                header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Total").Bold().FontColor(Colors.White);
                            });

                            foreach (var detalle in factura.Detalles)
                            {
                                table.Cell().Element(BodyCellStyle).Text(detalle.Producto);
                                table.Cell().Element(BodyCellStyle).AlignCenter().Text(detalle.Cantidad.ToString());
                                table.Cell().Element(BodyCellStyle).AlignRight().Text($"${detalle.PrecioUnitario:F2}");
                                table.Cell().Element(BodyCellStyle).AlignRight().Text($"${detalle.TotalLinea:F2}");
                            }
                        });

                        column.Item().AlignRight().Width(240).Background(Colors.Grey.Lighten4).Padding(12).Column(resumen =>
                        {
                            resumen.Spacing(7);

                            resumen.Item().Text("RESUMEN")
                                .Bold()
                                .FontColor(rojoUTA)
                                .FontSize(12);

                            resumen.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Subtotal:");
                                row.ConstantItem(90).AlignRight().Text($"${factura.Subtotal:F2}");
                            });

                            resumen.Item().Row(row =>
                            {
                                row.RelativeItem().Text("IVA (15%):");
                                row.ConstantItem(90).AlignRight().Text($"${factura.Iva:F2}");
                            });

                            resumen.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                            resumen.Item().Row(row =>
                            {
                                row.RelativeItem().Text("TOTAL:")
                                    .Bold()
                                    .FontSize(13)
                                    .FontColor(rojoUTA);

                                row.ConstantItem(90).AlignRight().Text($"${factura.Total:F2}")
                                    .Bold()
                                    .FontSize(13)
                                    .FontColor(rojoUTA);
                            });
                        });
                    });

                    page.Footer().PaddingTop(10).Column(footer =>
                    {
                        footer.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        footer.Item().PaddingTop(5).AlignCenter().Text(text =>
                        {
                            text.Span("UTA Tech - Documento generado por el sistema")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                        });
                    });
                });
            }).GeneratePdf();

            return pdf;
        }

        static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Background("#A30000")
                .Border(1)
                .BorderColor(Colors.White)
                .PaddingVertical(8)
                .PaddingHorizontal(10);
        }

        static IContainer BodyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(8)
                .PaddingHorizontal(10);
        }
    }
}