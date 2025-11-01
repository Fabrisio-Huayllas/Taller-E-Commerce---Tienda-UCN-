using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.ProductDTO
{
    /// <summary>
    /// DTO utilizado para crear un nuevo producto.
    /// </summary>
    /// <remarks>
    /// Contiene información del producto como título, descripción, precio, descuento, stock, estado, categoría, marca e imágenes.
    /// Aplica validaciones de requerimiento, rango y longitud de caracteres.
    /// </remarks>
    public class CreateProductDTO
    {
        /// <summary>
        /// Título o nombre del producto.
        /// </summary>
        [Required(ErrorMessage = "El título del producto es obligatorio.")]
        [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres.")]
        [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres.")]
        public required string Title { get; set; }

        /// <summary>
        /// Descripción del producto.
        /// </summary>
        [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres.")]
        public required string Description { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        [Required(ErrorMessage = "El precio del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public required int Price { get; set; }

        /// <summary>
        /// Descuento aplicado al producto (0 a 100).
        /// </summary>
        [Required(ErrorMessage = "El descuento del producto es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
        public required int Discount { get; set; }

        /// <summary>
        /// Cantidad disponible en stock.
        /// </summary>
        [Required(ErrorMessage = "El stock del producto es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un valor positivo.")]
        public required int Stock { get; set; }

        /// <summary>
        /// Estado del producto: "New" o "Used".
        /// </summary>
        [Required(ErrorMessage = "El estado del producto es obligatorio.")]
        [RegularExpression("^(New|Used)$", ErrorMessage = "El estado debe ser 'Nuevo' o 'Usado'.")]
        public required string Status { get; set; }

        /// <summary>
        /// ID de la categoría del producto.
        /// </summary>
        [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
        public required int CategoryId { get; set; }

        /// <summary>
        /// ID de la marca del producto.
        /// </summary>
        [Required(ErrorMessage = "El ID de la marca es obligatorio.")]
        public required int BrandId { get; set; }

        /// <summary>
        /// Lista de imágenes del producto.
        /// </summary>
        [Required(ErrorMessage = "Las imágenes del producto son obligatorias.")]
        public required List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}