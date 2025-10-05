using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.AuthDTO
{
    /// <summary>
    /// DTO utilizado para actualizar la información del perfil de un usuario.
    /// </summary>
    /// <remarks>
    /// Contiene los campos opcionales que se pueden modificar, como nombre, apellido y número de teléfono.
    /// </remarks>
    public class UpdateProfileDTO
    {
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