namespace TiendaProyecto.src.Application.DTO.BrandDTO
{
    /// <summary>
    /// DTO que representa un listado paginado de marcas.
    /// </summary>
    public class ListedBrandsDTO
    {
        /// <summary>
        /// Lista de marcas.
        /// </summary>
        public List<BrandDetailDTO> Brands { get; set; } = new List<BrandDetailDTO>();

        /// <summary>
        /// Cantidad total de marcas.
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
        /// Cantidad de marcas por página.
        /// </summary>
        public int PageSize { get; set; }
    }
}