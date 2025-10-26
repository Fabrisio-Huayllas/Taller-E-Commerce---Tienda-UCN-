using TiendaProyecto.src.Application.DTO.BrandDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de marcas.
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// Obtiene todas las marcas con paginación.
        /// </summary>
        Task<ListedBrandsDTO> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una marca por ID.
        /// </summary>
        Task<BrandDetailDTO> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva marca.
        /// </summary>
        Task<BrandDetailDTO> CreateAsync(BrandCreateDTO createDto);

        /// <summary>
        /// Actualiza una marca existente.
        /// </summary>
        Task<BrandDetailDTO> UpdateAsync(int id, BrandUpdateDTO updateDto);

        /// <summary>
        /// Elimina una marca.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}