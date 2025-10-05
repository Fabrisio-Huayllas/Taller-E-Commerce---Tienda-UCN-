using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.AuthDTO
{
    public class LoginDTO
    {
         /// <summary>
        /// Email del usuario.
        /// </summary>
        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
        public required string Email { get; set; }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida.")]
        public required string Password { get; set; }

        /// <summary>
        /// Indica si se debe recordar al usuario.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}