namespace TiendaProyecto.src.Application.DTO.CartDTO
{
    /// <summary>
    /// DTO que representa un carrito de compras de un usuario.
    /// </summary>
    /// <remarks>
    /// Contiene información del comprador, lista de ítems en el carrito, subtotal y total del carrito.
    /// </remarks>
    public class CartDTO
    {
        /// <summary>
    /// Identificador único del comprador (puede ser anónimo o registrado).
    /// </summary>
        public required string BuyerId { get; set; }

        /// <summary>
    /// Identificador del usuario registrado (opcional si el comprador es anónimo).
    /// </summary>
        public required int? UserId { get; set; }

        /// <summary>
    /// Lista de productos contenidos en el carrito.
    /// </summary>
        public required List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();

        /// <summary>
        /// Precio subtotal del carrito.
        /// </summary>
        public required string SubTotalPrice { get; set; }

        /// <summary>
        /// Precio total del carrito.
        /// </summary>
        public required string TotalPrice { get; set; }
    }
}