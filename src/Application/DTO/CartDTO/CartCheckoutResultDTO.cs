using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.CartDTO
{
    public class CartCheckoutResultDTO
    {
        /// <summary>
        /// Carrito actualizado después de validaciones.
        /// </summary>
        public required CartDTO Cart { get; set; }
        
        /// <summary>
        /// Lista de advertencias sobre cambios realizados.
        /// </summary>
        public List<string> Warnings { get; set; } = new();
        
        /// <summary>
        /// Indica si hubo alguna advertencia.
        /// </summary>
        public bool HasWarnings { get; set; }
    }
}