namespace TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO
{
    /// <summary>
    /// DTO que representa un listado paginado de productos para clientes.
    /// </summary>
    /// <remarks>
    /// Contiene la lista de productos visibles para el cliente, total de registros, total de páginas, página actual y tamaño de página.
    /// </remarks>
    public class ListedProductsForCustomerDTO
    {
        /// <summary>
        /// Lista de productos disponibles para el cliente.
        /// </summary>
        public List<ProductForCustomerDTO> Products { get; set; } = new List<ProductForCustomerDTO>();

        /// <summary>
        /// Cantidad total de productos disponibles.
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
