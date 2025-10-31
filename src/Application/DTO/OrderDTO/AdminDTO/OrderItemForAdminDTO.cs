using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.OrderItemDTO
{
    /// <summary>
    /// DTO que representa un ítem de pedido para el panel de administración.
    /// </summary>
    public class OrderItemForAdminDTO
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public required string ProductName { get; set; }

        /// <summary>
        /// Precio unitario del producto (al momento de la compra).
        /// </summary>
        public required int UnitPrice { get; set; }

        /// <summary>
        /// Cantidad pedida del producto.
        /// </summary>
        public required int Quantity { get; set; }

        /// <summary>
        /// Subtotal calculado para este ítem (UnitPrice * Quantity).
        /// </summary>
        public required int SubTotal { get; set; }

        /// <summary>
        /// URL de la imagen principal del producto.
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}