using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.AuthDTO
{

    /// <summary>
/// DTO que representa la información del perfil de un usuario.
/// </summary>
/// <remarks>
/// Contiene datos básicos del usuario como ID, correo electrónico, nombre, apellido y número de teléfono.
/// </remarks>
    public class UserProfileDTO
    {
        /// <summary>
    /// Identificador único del usuario.
    /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Número de teléfono del usuario.
        /// </summary>
        public string? PhoneNumber { get; set; }
    }
}