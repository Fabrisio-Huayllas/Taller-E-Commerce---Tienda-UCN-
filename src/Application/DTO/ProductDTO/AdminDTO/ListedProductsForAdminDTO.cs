using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.DTO.ProductDTO.AdminDTO
{
    /// <summary>
    /// DTO que representa un listado paginado de productos para el panel de administración.
    /// </summary>
    /// <remarks>
    /// Contiene la lista de productos, total de registros, total de páginas, página actual y tamaño de página.
    /// </remarks>
    public class ListedProductsForAdminDTO
    {
        /// <summary>
        /// Lista de productos disponibles para administración.
        /// </summary>
        public List<ProductForAdminDTO> Products { get; set; } = new List<ProductForAdminDTO>();

        /// <summary>
        /// Cantidad total de productos.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Número total de páginas disponibles.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Página actual del listado.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Cantidad de productos por página.
        /// </summary>
        public int PageSize { get; set; }
    }
}