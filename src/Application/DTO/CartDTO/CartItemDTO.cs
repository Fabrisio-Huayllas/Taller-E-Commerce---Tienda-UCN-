namespace TiendaProyecto.src.Application.DTO.CartDTO
{
    /// <summary>
/// DTO que representa un ítem dentro de un carrito de compras.
/// </summary>
/// <remarks>
/// Contiene información del producto, cantidad, precio, descuento y los totales asociados.
/// </remarks>
    public class CartItemDTO
    {
        /// <summary>
    /// Identificador único del producto.
    /// </summary>
        public required int ProductId { get; set; }

        /// <summary>
        /// Título del producto.
        /// </summary>
        public required string ProductTitle { get; set; }

        /// <summary>
        /// URL de la imagen del producto.
        /// </summary>
        public required string ProductImageUrl { get; set; }
        /// <summary>
    /// Precio unitario del producto.
    /// </summary>
        public required int Price { get; set; }

        /// <summary>
        /// Cantidad del producto en el carrito.
        /// </summary>
        public required int Quantity { get; set; }

        /// <summary>
        /// Descuento aplicado al producto.
        /// </summary>
        public required int Discount { get; set; }

        /// <summary>
        /// Precio subtotal del ítem en el carrito.
        /// </summary>
        public required string SubTotalPrice { get; set; }

        /// <summary>
        /// Precio total del ítem en el carrito.
        /// </summary>
        public required string TotalPrice { get; set; }
    }
}