using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// DTO utilizado para actualizar una categoría existente.
    /// </summary>
    public class CategoryUpdateDTO
    {
        /// <summary>
        /// Nombre de la categoría.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción opcional de la categoría.
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }
    }
}