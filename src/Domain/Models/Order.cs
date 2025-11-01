using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Enums;

namespace TiendaProyecto.src.Domain.Models
{
    /// <summary>
    /// Representa un pedido dentro del sistema.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        /// <summary>
        /// Código único legible del pedido
        /// </summary>
        public required string Code { get; set; }

        // Totales (asegurar existen; required según tu modelado)
        public int SubTotal { get; set; }
        public int Total { get; set; }

        /// <summary>
        /// ID del usuario cliente asociado al pedido.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Cliente que realizó el pedido.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Colección de ítems asociados a este pedido.
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Fecha y hora de creación del pedido.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última modificación del pedido.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //Campos ´para administracion

        /// <summary>
        /// Estado actual del pedido (Created, Paid, Shipped, etc.).
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        /// <summary>
        /// Fecha del último cambio de estado.
        /// </summary>
        public DateTime? StatusChangedAt { get; set; }

        /// <summary>
        /// Id del administrador que realizó el último cambio de estado.
        /// </summary>
        public int? ChangedByAdminId { get; set; }

        /// <summary>
        /// Notas / motivo del cambio de estado y pequeño historial.
        /// </summary>
        public string? ChangeReason { get; set; }
    }
}