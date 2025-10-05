namespace TiendaProyecto.src.Application.DTO.OrderDTO
{
    /// <summary>
    /// DTO que representa el detalle de una orden de compra.
    /// </summary>
    /// <remarks>
    /// Contiene información de la orden como código, subtotal, total, fecha de compra y los ítems asociados.
    /// </remarks>
    public class OrderDetailDTO
    {
        /// <summary>
        /// Código único de la orden.
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Precio total de la orden.
        /// </summary>
        public required string Total { get; set; }
        /// <summary>
        /// Subtotal de la orden antes de impuestos o descuentos.
        /// </summary>
        public required string SubTotal { get; set; }
        /// <summary>
        /// Fecha y hora en que se realizó la compra.
        /// </summary>
        public required DateTime PurchasedAt { get; set; }
        /// <summary>
        /// Lista de ítems que forman parte de la orden.
        /// </summary>
        public required List<OrderItemDTO> Items { get; set; }
    }
}