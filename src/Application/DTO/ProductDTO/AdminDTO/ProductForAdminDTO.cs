using Bogus.DataSets;

namespace TiendaProyecto.src.Application.DTO.ProductDTO.AdminDTO
{
    /// <summary>
    /// DTO que representa la información de un producto para el panel de administración.
    /// </summary>
    /// <remarks>
    /// Contiene datos básicos del producto, su disponibilidad, categoría, marca, estado y fecha de última actualización.
    /// </remarks>
    public class ProductForAdminDTO
    {
        public required int Id { get; set; }
        /// <summary>
        /// Título o nombre del producto.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// URL de la imagen principal del producto.
        /// </summary>
        public string? MainImageURL { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public required string Price { get; set; }

        /// <summary>
        /// Cantidad de unidades disponibles en stock.
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

        /// <summary>
        /// Fecha y hora de la última actualización del producto.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }
        public String FinalPrice { get; set; }
    }
}