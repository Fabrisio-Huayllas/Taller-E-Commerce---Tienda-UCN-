using System.ComponentModel.DataAnnotations;

namespace TiendaProyecto.src.Application.DTO.ProductDTO
{
    /// <summary>
    /// DTO que representa los parámetros para la búsqueda y paginación de productos.
    /// </summary>
    /// <remarks>
    /// Contiene el número de página, tamaño de página y un término de búsqueda opcional con validaciones.
    /// </remarks>
    public class SearchParamsDTO
    {
        /// <summary>
        /// Número de página que se desea consultar.
        /// </summary>
        [Required(ErrorMessage = "El número de página es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser un número entero positivo.")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Tamaño de página que se desea consultar.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "El tamaño de página debe ser un número entero positivo.")]
        public int? PageSize { get; set; }

        /// <summary>
        /// Término de búsqueda para filtrar productos. Opcional.
        /// </summary>

        [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
        [MaxLength(40, ErrorMessage = "El término de búsqueda no puede exceder los 40 caracteres.")]
        public string? SearchTerm { get; set; }
    }
}