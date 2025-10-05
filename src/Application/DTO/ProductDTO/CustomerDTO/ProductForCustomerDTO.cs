using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO
{
    /// <summary>
    /// DTO que representa la información básica de un producto visible para el cliente.
    /// </summary>
    /// <remarks>
    /// Contiene título, descripción, imagen principal, precio y descuento del producto.
    /// </remarks>
    public class ProductForCustomerDTO
    {
        /// <summary>
        /// Título o nombre del producto.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Descripción breve del producto.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// URL de la imagen principal del producto.
        /// </summary>
        public required string MainImageURL { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public required string Price { get; set; }

        /// <summary>
        /// Descuento aplicado al producto, si corresponde.
        /// </summary>
        public required int Discount { get; set; }
    }
}