using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.CartDTO
{
   /// <summary>
    /// DTO utilizado para agregar un producto al carrito de compras.
    /// </summary>
    /// <remarks>
    /// Contiene el ID del producto y la cantidad deseada, ambos con validación de requerimiento y valores positivos.
    /// </remarks>
    public class AddCartItemDTO
    {
         /// <summary>
        /// Identificador del producto que se agregará al carrito.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public required int ProductId { get; set; }

        /// <summary>
        /// Cantidad del producto a agregar al carrito.
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public required int Quantity { get; set; }
    }
}