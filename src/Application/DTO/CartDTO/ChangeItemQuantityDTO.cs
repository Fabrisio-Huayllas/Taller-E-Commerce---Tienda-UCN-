using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.CartDTO
{
    /// <summary>
    /// DTO utilizado para cambiar la cantidad de un ítem en el carrito de compras.
    /// </summary>
    /// <remarks>
    /// Contiene el ID del producto y la nueva cantidad deseada, ambos con validación de requerimiento y valores positivos.
    /// </remarks>
    public class ChangeItemQuantityDTO
    {
        /// <summary>
        /// Identificador del producto cuyo valor se desea actualizar.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public required int ProductId { get; set; }
         /// <summary>
        /// Nueva cantidad del producto en el carrito.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public required int Quantity { get; set; }
    }
}