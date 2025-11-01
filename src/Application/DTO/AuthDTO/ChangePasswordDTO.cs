using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TiendaProyecto.src.Application.DTO.AuthDTO
{
    /// <summary>
    /// DTO utilizado para cambiar la contraseña de un usuario.
    /// </summary>
    /// <remarks>
    /// Contiene los campos necesarios para validar y actualizar la contraseña actual por una nueva.
    /// </remarks>
    public class ChangePasswordDTO
    {
        /// <summary>
        /// Contraseña actual del usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public string CurrentPassword { get; set; } = null!;

        /// <summary>
        /// Nueva contraseña que reemplazará a la actual.
        /// </summary>
        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
        
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        
        public string NewPassword { get; set; } = null!;

       /// <summary>
        /// Confirmación de la nueva contraseña para verificar coincidencia.
        /// </summary>
        [Compare("NewPassword", ErrorMessage = "La confirmación de la nueva contraseña no coincide.")]
        public string? ConfirmNewPassword { get; set; }
    }
}