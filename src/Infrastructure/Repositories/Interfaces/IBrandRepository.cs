using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de marcas.
    /// </summary>
    public interface IBrandRepository
    {
        /// <summary>
        /// Obtiene todas las marcas con paginación y búsqueda.
        /// </summary>
        Task<(IEnumerable<Brand> brands, int totalCount)> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una marca por ID.
        /// </summary>
        Task<Brand?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una marca por slug.
        /// </summary>
        Task<Brand?> GetBySlugAsync(string slug);

        /// <summary>
        /// Verifica si existe una marca con el nombre especificado.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>
        /// Verifica si existe una marca con el slug especificado.
        /// </summary>
        Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null);

        /// <summary>
        /// Crea una nueva marca.
        /// </summary>
        Task<Brand> CreateAsync(Brand brand);

        /// <summary>
        /// Actualiza una marca existente.
        /// </summary>
        Task<Brand> UpdateAsync(Brand brand);

        /// <summary>
        /// Elimina una marca (soft delete).
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Cuenta productos asociados a una marca.
        /// </summary>
        Task<int> CountProductsByBrand(int brandId);

        /// <summary>
        /// Genera un slug único basado en el nombre.
        /// </summary>
        Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null);
    }
}