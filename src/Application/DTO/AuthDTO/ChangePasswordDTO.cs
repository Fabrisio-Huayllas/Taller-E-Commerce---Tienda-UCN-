using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TiendaProyecto.src.Application.DTO.AuthDTO
{
    public class ChangePasswordDTO
    {
         [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
        public string NewPassword { get; set; } = null!;
        
        [Compare("NewPassword", ErrorMessage = "La confirmación de la nueva contraseña no coincide.")]
        public string? ConfirmNewPassword { get; set; }
    }
}