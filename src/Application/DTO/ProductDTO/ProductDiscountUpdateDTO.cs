using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TiendaProyecto.src.Application.DTO.ProductDTO
{
    public class ProductDiscountUpdateDTO
    {
         [Required(ErrorMessage = "El porcentaje de descuento es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100.")]
        public int DiscountPercent { get; set; }
    }
}