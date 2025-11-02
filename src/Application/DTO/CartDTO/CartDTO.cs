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

        /// <summary>
        /// Ahorro total generado por descuentos aplicados (SubTotal - Total).
        /// </summary>
        public required string Savings { get; set; } // ✅ AGREGAR ESTA LÍNEA
        
        /// <summary>
        /// Cantidad total de items en el carrito.
        /// </summary>
        public required int TotalQuantity { get; set; } // ✅ AGREGAR ESTA LÍNEA
    
    }
}