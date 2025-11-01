using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.OrderItemDTO;

namespace TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO
{
    /// <summary>
    /// DTO que representa el detalle completo de un pedido para el panel de administración.
    /// </summary>
    /// <remarks>
    /// Incluye los datos del cliente, los ítems del pedido, su estado y totales.
    /// </remarks>
    public class OrderDetailForAdminDTO
    {
        /// <summary>
        /// Código único del pedido.
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Fecha en que se creó el pedido.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Estado actual del pedido (como string para presentación).
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Nombre del cliente.
        /// </summary>
        public required string CustomerFirstName { get; set; }
        /// <summary>
        /// Apellido del cliente.
        /// </summary>
        public required string CustomerLastName { get; set; }
        /// <summary>
        /// Correo electrónico del cliente.
        /// </summary>
        public required string CustomerEmail { get; set; }

        /// <summary>
        /// Monto total del pedido.
        /// </summary>
        public required int Total { get; set; }

        /// <summary>
        /// Dirección de envío asociada al pedido (si aplica).
        /// </summary>
        public string? ShippingAddress { get; set; }

        /// <summary>
        /// Subtotal antes de impuestos o descuentos.
        /// </summary>
        public required int SubTotal { get; set; }

        /// <summary>
        /// Lista de productos incluidos en el pedido.
        /// </summary>
        public List<OrderItemForAdminDTO> Items { get; set; } = new List<OrderItemForAdminDTO>();

        // Campos de auditoría/estado
        /// <summary>
        /// Fecha del último cambio de estado.
        /// </summary>
        public DateTime? StatusChangedAt { get; set; }

        /// <summary>
        /// ID del administrador que realizó el último cambio de estado.
        /// </summary>
        public int? ChangedByAdminId { get; set; }

        /// <summary>
        /// Razón o nota asociada al último cambio de estado.
        /// </summary>
        public string? ChangeReason { get; set; }
        public string TotalFormatted =>
            Total.ToString("C0", new System.Globalization.CultureInfo("es-CL"));

        public string SubTotalFormatted =>
            SubTotal.ToString("C0", new System.Globalization.CultureInfo("es-CL"));
    }
}