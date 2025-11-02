using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.UserDTO
{
    /// <summary>
    /// DTO para actualizar el estado de un usuario (activo/bloqueado).
    /// </summary>
    public class UpdateUserStatusDTO
    {
        /// <summary>
        /// Estado del usuario: "active" o "blocked".
        /// </summary>
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("^(active|blocked)$", ErrorMessage = "El estado debe ser 'active' o 'blocked'.")]
        public required string Status { get; set; }

        /// <summary>
        /// Razón opcional para el cambio de estado.
        /// </summary>
        [StringLength(500, ErrorMessage = "La razón no puede exceder los 500 caracteres.")]
        public string? Reason { get; set; }
    }
}