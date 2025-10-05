namespace TiendaProyecto.src.Application.DTO.OrderDTO
{
    /// <summary>
    /// DTO que representa un listado paginado de órdenes con sus detalles.
    /// </summary>
    /// <remarks>
    /// Contiene la lista de órdenes, el total de registros, el total de páginas, la página actual y el tamaño de página.
    /// </remarks>
    public class ListedOrderDetailDTO
    {
        /// <summary>
        /// Lista de detalles de órdenes.
        /// </summary>
        public required List<OrderDetailDTO> Orders { get; set; } = new List<OrderDetailDTO>();
        /// <summary>
        /// Cantidad total de órdenes.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total de páginas disponibles.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Página actual.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Tamaño de página.
        /// </summary>
        public int PageSize { get; set; }
    }
}