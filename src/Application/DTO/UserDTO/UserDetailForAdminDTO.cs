using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.UserDTO
{
    /// <summary>
    /// DTO que representa el detalle completo de un usuario para administradores.
    /// </summary>
    public class UserDetailForAdminDTO
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// RUT del usuario.
        /// </summary>
        public required string Rut { get; set; }

        /// <summary>
        /// Número de teléfono del usuario.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public required string Role { get; set; }

        /// <summary>
        /// Estado del usuario (Activo/Bloqueado).
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Indica si el email ha sido confirmado.
        /// </summary>
        public required bool EmailConfirmed { get; set; }

        /// <summary>
        /// Fecha de registro del usuario.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Fecha del último acceso (opcional).
        /// </summary>
        public DateTime? LastLogin { get; set; }
    }
}