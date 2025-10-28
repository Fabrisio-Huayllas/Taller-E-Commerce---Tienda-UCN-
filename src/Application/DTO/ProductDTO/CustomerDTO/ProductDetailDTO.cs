namespace TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO
{
    /// <summary>
    /// DTO que representa el detalle completo de un producto para el cliente.
    /// </summary>
    /// <remarks>
    /// Contiene información del producto, imágenes, precio, descuento, stock, categoría, marca, estado y disponibilidad.
    /// </remarks>
    public class ProductDetailDTO
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Título o nombre del producto.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Descripción detallada del producto.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Lista de URLs de imágenes del producto.
        /// </summary>
        public List<string> ImagesURL { get; set; } = new List<string>();

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public required string Price { get; set; }

        /// <summary>
        /// Descuento aplicado al producto, si corresponde.
        /// </summary>
        public required int Discount { get; set; }

        /// <summary>
        /// Cantidad disponible en stock.
        /// </summary>
        public required int Stock { get; set; }

        /// <summary>
        /// Indicador del nivel de stock (por ejemplo: Bajo, Medio, Alto).
        /// </summary>
        public required string StockIndicator { get; set; }

        /// <summary>
        /// Nombre de la categoría a la que pertenece el producto.
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// Nombre de la marca del producto.
        /// </summary>
        public required string BrandName { get; set; }

        /// <summary>
        /// Estado del producto (por ejemplo: Activo, Inactivo).
        /// </summary>
        public required string StatusName { get; set; }

        /// <summary>
        /// Indica si el producto está disponible para la venta.
        /// </summary>
        public required bool IsAvailable { get; set; }

        public String FinalPrice { get; set; }
        
    }
}