using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TiendaProyecto.src.Application.DTO.ProductDTO
{
    public class UpdateProductDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria.")]
        public int BrandId { get; set; }
    }
}