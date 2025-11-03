using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Domain.Models
{
    public class UserStatusAudit
    {
        /// <summary>
        /// Identificador único del registro de auditoría.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del usuario cuyo estado fue modificado.
        /// </summary>
        public required int UserId { get; set; }

        /// <summary>
        /// Usuario cuyo estado fue modificado.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// ID del administrador que realizó el cambio.
        /// </summary>
        public required int ChangedByAdminId { get; set; }

        /// <summary>
        /// Estado anterior del usuario.
        /// </summary>
        public required string PreviousStatus { get; set; }

        /// <summary>
        /// Nuevo estado del usuario.
        /// </summary>
        public required string NewStatus { get; set; }

        /// <summary>
        /// Razón del cambio de estado.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Fecha y hora del cambio.
        /// </summary>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}