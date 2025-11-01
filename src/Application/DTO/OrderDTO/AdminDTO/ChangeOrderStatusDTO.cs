using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Enums;
using System.Text.Json.Serialization;

namespace TiendaProyecto.src.Application.DTO.OrderDTO
{
    public class ChangeOrderStatusDTO
    {
        /// <summary>
        /// Nuevo estado para la orden.
        /// </summary>
        [JsonPropertyName("status")]
    public required OrderStatus NewStatus { get; set; }

        /// <summary>
        /// Motivo del cambio (opcional, pero recomendado para auditoría).
        /// </summary>
        [JsonPropertyName("note")]
    public string? Reason { get; set; }
    }
}