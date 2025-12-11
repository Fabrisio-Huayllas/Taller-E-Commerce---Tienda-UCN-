using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TiendaProyecto.src.Application.DTO.OrderDTO;

namespace TiendaProyecto.src.Application.Services.Implements
{
    public class OrderPdfDocument
    {
        private readonly OrderDetailDTO _order;

        public OrderPdfDocument(OrderDetailDTO order)
        {
            _order = order;
            
            // Configurar licencia gratuita de QuestPDF para uso no comercial
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePdf()
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Pagina ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Tienda UCN").Bold().FontSize(20).FontColor(Colors.Blue.Medium);
                    column.Item().Text("Universidad Catolica del Norte");
                    column.Item().Text("Antofagasta, Chile");
                });

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text($"Orden #{_order.Code}").Bold().FontSize(16);
                    column.Item().Text($"Fecha: {_order.PurchasedAt:dd/MM/yyyy HH:mm}");
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                // Tabla de productos
                column.Item().Element(ComposeProductsTable);

                // Resumen de totales
                column.Item().Element(ComposeSummary);
            });
        }

        void ComposeProductsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Producto
                    columns.RelativeColumn(1); // Cantidad
                    columns.RelativeColumn(1); // Precio Unitario
                    columns.RelativeColumn(1); // Subtotal
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Producto").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Cantidad").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").Bold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Background(Colors.Blue.Lighten3)
                            .Padding(5);
                    }
                });

                // Items
                foreach (var item in _order.Items)
                {
                    var totalPrice = item.PriceAtMoment * item.Quantity;
                    
                    table.Cell().Element(CellStyle).Text(item.ProductTitle);
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"${item.PriceAtMoment:N0}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"${totalPrice:N0}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(5);
                    }
                }
            });
        }

        void ComposeSummary(IContainer container)
        {
            var totalQuantity = _order.Items.Sum(i => i.Quantity);
            
            container.AlignRight().Column(column =>
            {
                column.Spacing(5);
                column.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Total de Items:").Bold();
                    row.AutoItem().Width(100).AlignRight().Text(totalQuantity.ToString());
                });
                
                column.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Subtotal:").Bold();
                    row.AutoItem().Width(100).AlignRight().Text($"${_order.SubTotal:N0}");
                });
                
                column.Item().Row(row =>
                {
                    row.AutoItem().Width(150).Text("Total:").Bold().FontSize(14).FontColor(Colors.Blue.Medium);
                    row.AutoItem().Width(100).AlignRight().Text($"${_order.Total:N0}").Bold().FontSize(14).FontColor(Colors.Blue.Medium);
                });
            });
        }

    }
}
