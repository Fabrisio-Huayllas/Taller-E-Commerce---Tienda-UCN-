using Mapster;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.CategoryDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para la gestión de categorías.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public CategoryService(ICategoryRepository categoryRepository, IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<ListedCategoriesDTO> GetAllAsync(SearchParamsDTO searchParams)
        {
            var (categories, totalCount) = await _categoryRepository.GetAllAsync(searchParams);
            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var categoryDtos = new List<CategoryDetailDTO>();
            foreach (var category in categories)
            {
                var productCount = await _categoryRepository.CountProductsByCategory(category.Id);
                var dto = category.Adapt<CategoryDetailDTO>();
                dto.ProductCount = productCount;
                categoryDtos.Add(dto);
            }

            return new ListedCategoriesDTO
            {
                Categories = categoryDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = searchParams.PageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CategoryDetailDTO> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Categoría con ID {id} no encontrada.");
            }

            var productCount = await _categoryRepository.CountProductsByCategory(id);
            var dto = category.Adapt<CategoryDetailDTO>();
            dto.ProductCount = productCount;

            return dto;
        }

        public async Task<CategoryDetailDTO> CreateAsync(CategoryCreateDTO createDto)
        {
            // Sanitizar datos de entrada
            var sanitizedName = SanitizeInput(createDto.Name);
            var sanitizedDescription = !string.IsNullOrEmpty(createDto.Description)
                ? SanitizeInput(createDto.Description)
                : null;

            // Verificar unicidad del nombre
            if (await _categoryRepository.ExistsByNameAsync(sanitizedName))
            {
                throw new ConflictException($"Ya existe una categoría con el nombre '{sanitizedName}'.");
            }

            // Crear categoría usando mapper y luego establecer campos específicos
            var category = createDto.Adapt<Category>();
            category.Name = sanitizedName;
            category.Description = sanitizedDescription;
            category.Slug = await _categoryRepository.GenerateUniqueSlugAsync(sanitizedName);

            var createdCategory = await _categoryRepository.CreateAsync(category);
            Log.Information("Categoría creada: {@Category}", createdCategory);

            var dto = createdCategory.Adapt<CategoryDetailDTO>();
            dto.ProductCount = 0;

            return dto;
        }

        public async Task<CategoryDetailDTO> UpdateAsync(int id, CategoryUpdateDTO updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Categoría con ID {id} no encontrada.");
            }

            // Sanitizar datos de entrada
            var sanitizedName = SanitizeInput(updateDto.Name);
            var sanitizedDescription = !string.IsNullOrEmpty(updateDto.Description)
                ? SanitizeInput(updateDto.Description)
                : null;

            // Verificar unicidad del nombre (excluyendo la categoría actual)
            if (await _categoryRepository.ExistsByNameAsync(sanitizedName, id))
            {
                throw new ConflictException($"Ya existe otra categoría con el nombre '{sanitizedName}'.");
            }

            // Generar nuevo slug si el nombre cambió
            string newSlug = category.Slug;
            if (category.Name != sanitizedName)
            {
                newSlug = await _categoryRepository.GenerateUniqueSlugAsync(sanitizedName, id);
            }

            category.Name = sanitizedName;
            category.Slug = newSlug;
            category.Description = sanitizedDescription;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            var productCount = await _categoryRepository.CountProductsByCategory(id);

            var dto = updatedCategory.Adapt<CategoryDetailDTO>();
            dto.ProductCount = productCount;

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Categoría con ID {id} no encontrada.");
            }

            // Verificar si hay productos asociados
            var productCount = await _categoryRepository.CountProductsByCategory(id);
            if (productCount > 0)
            {
                throw new ConflictException(
                    $"No se puede eliminar la categoría '{category.Name}' porque tiene {productCount} producto(s) asociado(s). " +
                    "Elimine o reasigne los productos antes de eliminar la categoría."
                );
            }

            return await _categoryRepository.DeleteAsync(id);
        }

        private static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remover HTML y scripts
            var sanitized = Regex.Replace(input, @"<[^>]*>", string.Empty);
            sanitized = Regex.Replace(sanitized, @"<script[^>]*>.*?</script>", string.Empty, RegexOptions.IgnoreCase);

            return sanitized.Trim();
        }
    }
}