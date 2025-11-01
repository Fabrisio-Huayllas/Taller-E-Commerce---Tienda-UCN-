using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Enums;

namespace TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO
{
    /// <summary>
    /// DTO utilizado para actualizar el estado de un pedido desde el panel de administración.
    /// </summary>
    public class UpdateOrderStatusDTO
    {
        /// <summary>
        /// Nuevo estado que se asignará al pedido.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Motivo o nota opcional asociada al cambio de estado.
        /// </summary>
        public string? Note { get; set; }
    }
}
