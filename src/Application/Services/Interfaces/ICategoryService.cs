using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.CategoryDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de categorías.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Obtiene todas las categorías con paginación.
        /// </summary>
        Task<ListedCategoriesDTO> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una categoría por ID.
        /// </summary>
        Task<CategoryDetailDTO> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        Task<CategoryDetailDTO> CreateAsync(CategoryCreateDTO createDto);

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        Task<CategoryDetailDTO> UpdateAsync(int id, CategoryUpdateDTO updateDto);

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}