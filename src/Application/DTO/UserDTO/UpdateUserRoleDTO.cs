using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.UserDTO
{
    public class UpdateUserRoleDTO
    {
        [Required(ErrorMessage = "El rol es requerido")]
        [RegularExpression("^(Admin|Customer)$", ErrorMessage = "El rol debe ser 'Cliente' o 'Administrador'")]
        public string Role { get; set; } = string.Empty;
    }
}