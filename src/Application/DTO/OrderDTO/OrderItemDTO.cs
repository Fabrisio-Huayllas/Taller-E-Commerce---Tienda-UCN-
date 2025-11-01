namespace TiendaProyecto.src.Application.DTO.OrderDTO
{
    /// <summary>
    /// DTO que representa un ítem dentro de una orden de compra.
    /// </summary>
    /// <remarks>
    /// Contiene información del producto, descripción, imagen principal, precio al momento de la compra y cantidad.
    /// </remarks>
    public class OrderItemDTO
    {
        /// <summary>
        /// Título o nombre del producto.
        /// </summary>
        public required string ProductTitle { get; set; }
         /// <summary>
        /// Descripción del producto.
        /// </summary>
        public required string ProductDescription { get; set; }

        /// <summary>
        /// URL de la imagen principal del producto.
        /// </summary>
        public required string MainImageURL { get; set; }
        /// <summary>
        /// Precio del producto al momento de la compra.
        /// </summary>
        public required int PriceAtMoment { get; set; }
        /// <summary>
        /// Cantidad de unidades del producto en la orden.
        /// </summary>
        public required int Quantity { get; set; }
        /// <summary>
        /// formato CLP
        /// </summary> 
        public string PriceAtMomentFormatted =>
            PriceAtMoment.ToString("C0", new System.Globalization.CultureInfo("es-CL"));
    }
}