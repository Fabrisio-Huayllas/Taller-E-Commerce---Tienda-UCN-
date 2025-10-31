using System;
using System.Collections.Generic;
using TiendaProyecto.src.Application.DTO.OrderItemDTO;
namespace TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO
{
    /// <summary>
    /// DTO que representa una orden para listados en el panel de administración.
    /// </summary>
    public class OrderForAdminDTO
    {
        /// <summary>
        /// ID de la orden.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Código legible de la orden 
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Fecha de creación del pedido.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de la última modificación del pedido.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Estado actual de la orden.
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Datos de cliente
        /// </summary>
        public CustomerSummaryDTO Customer { get; set; } = new CustomerSummaryDTO();

        /// <summary>
        /// Correo electrónico del cliente.
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// ID del usuario cliente.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Conteo y detalle de ítems en la orden.
        /// </summary>
        public int ItemsCount { get; set; }
        public List<OrderItemForAdminDTO> Items { get; set; } = new List<OrderItemForAdminDTO>();

        /// <summary>
        /// Fecha del último cambio de estado.
        /// </summary>
        public DateTime? StatusChangedAt { get; set; }

        /// <summary>
        /// Razón del último cambio de estado.
        /// </summary>
        public string? ChangeReason { get; set; }

        /// <summary>
        /// Monto total del pedido en pesos chilenos (CLP)
        /// </summary>
        public int Total { get; set; } 
    }

    public class CustomerSummaryDTO
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}