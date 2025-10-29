using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.CategoryDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapear objetos de tipo DTO a Category y viceversa.
    /// </summary>
    public class CategoryMapper
    {
        public CategoryMapper() { }

        /// <summary>
        /// Configura todos los mapeos de categorías.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureCategoryMappings();
        }

        /// <summary>
        /// Configura el mapeo entre Category y sus DTOs.
        /// </summary>
        public void ConfigureCategoryMappings()
        {
            // Mapeo de Category a CategoryDetailDTO
            TypeAdapterConfig<Category, CategoryDetailDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Slug, src => src.Slug)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Ignore(dest => dest.ProductCount); // Se asigna manualmente en el servicio

            // Mapeo de CategoryCreateDTO a Category
            TypeAdapterConfig<CategoryCreateDTO, Category>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => true)
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Slug)
                .Ignore(dest => dest.Products);

            // Mapeo de CategoryUpdateDTO a Category (para actualizaciones)
            TypeAdapterConfig<CategoryUpdateDTO, Category>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Slug)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Products);
        }
    }
}