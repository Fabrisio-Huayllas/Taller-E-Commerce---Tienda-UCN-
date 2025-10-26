using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de categorías.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Obtiene todas las categorías con paginación y búsqueda.
        /// </summary>
        Task<(IEnumerable<Category> categories, int totalCount)> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una categoría por ID.
        /// </summary>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una categoría por slug.
        /// </summary>
        Task<Category?> GetBySlugAsync(string slug);

        /// <summary>
        /// Verifica si existe una categoría con el nombre especificado.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>
        /// Verifica si existe una categoría con el slug especificado.
        /// </summary>
        Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null);

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        Task<Category> CreateAsync(Category category);

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        Task<Category> UpdateAsync(Category category);

        /// <summary>
        /// Elimina una categoría (soft delete).
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Cuenta productos asociados a una categoría.
        /// </summary>
        Task<int> CountProductsByCategory(int categoryId);

        /// <summary>
        /// Genera un slug único basado en el nombre.
        /// </summary>
        Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null);
    }
}