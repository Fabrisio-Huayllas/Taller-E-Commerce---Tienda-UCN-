using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Enums;

namespace TiendaProyecto.src.Application.DTO.OrderDTO
{
    public class ChangeOrderStatusDTO
    {
        /// <summary>
        /// Nuevo estado para la orden.
        /// </summary>
        public required OrderStatus NewStatus { get; set; }

        /// <summary>
        /// Motivo del cambio (opcional, pero recomendado para auditoría).
        /// </summary>
        public string? Reason { get; set; }
    }
}