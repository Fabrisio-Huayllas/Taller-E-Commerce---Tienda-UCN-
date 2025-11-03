using System;
using TiendaProyecto.src.Domain.Enums;

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
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Tamaño de página que se desea consultar.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Término de búsqueda para filtrar productos. Opcional.
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Campo por el cual ordenar (ej. "createdAt", "total"). Validar en servicio.
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Dirección de ordenamiento: "asc" o "desc". Por defecto "desc".
        /// </summary>
        public string? OrderDir { get; set; } = "desc";

        // Filtros requeridos por R115
        public OrderStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? CustomerEmail { get; set; }
        public int? CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        
        /// <summary>
        /// Precio mínimo para filtrar productos. Opcional.
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Precio máximo para filtrar productos. Opcional.
        /// </summary>
        public decimal? MaxPrice { get; set; }
        /// <summary>
        /// ID de la categoría para filtrar productos. Opcional.
        /// </summary>
        public int? CategoryId { get; set; }
                /// <summary>
        /// ID de la marca para filtrar productos. Opcional.
        /// </summary>
        public int? BrandId { get; set; }
        
        /// <summary>
        /// Campo por el cual ordenar los productos. Opcional.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Dirección del ordenamiento (ascendente o descendente). Opcional.
        /// </summary>
        public string? SortDirection { get; set; }

    }
}