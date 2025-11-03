using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.BrandDTO
{
    /// <summary>
    /// DTO utilizado para crear una nueva marca.
    /// </summary>
    public class BrandCreateDTO
    {
        /// <summary>
        /// Nombre de la marca.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción opcional de la marca.
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }
    }
}