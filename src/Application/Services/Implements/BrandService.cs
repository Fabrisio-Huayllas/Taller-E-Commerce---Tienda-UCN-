using Mapster;
using Serilog;
using System.Text.RegularExpressions;
using TiendaProyecto.src.Application.DTO.BrandDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para la gestión de marcas.
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public BrandService(IBrandRepository brandRepository, IConfiguration configuration)
        {
            _brandRepository = brandRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<ListedBrandsDTO> GetAllAsync(SearchParamsDTO searchParams)
        {
            var (brands, totalCount) = await _brandRepository.GetAllAsync(searchParams);
            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var brandDtos = new List<BrandDetailDTO>();
            foreach (var brand in brands)
            {
                var productCount = await _brandRepository.CountProductsByBrand(brand.Id);
                var dto = brand.Adapt<BrandDetailDTO>();
                dto.ProductCount = productCount;
                brandDtos.Add(dto);
            }

            return new ListedBrandsDTO
            {
                Brands = brandDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = searchParams.PageNumber,
                PageSize = pageSize
            };
        }

        public async Task<BrandDetailDTO> GetByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new NotFoundException($"Marca con ID {id} no encontrada.");
            }

            var productCount = await _brandRepository.CountProductsByBrand(id);
            var dto = brand.Adapt<BrandDetailDTO>();
            dto.ProductCount = productCount;

            return dto;
        }

        public async Task<BrandDetailDTO> CreateAsync(BrandCreateDTO createDto)
        {
            // Sanitizar datos de entrada
            var sanitizedName = SanitizeInput(createDto.Name);
            var sanitizedDescription = !string.IsNullOrEmpty(createDto.Description)
                ? SanitizeInput(createDto.Description)
                : null;

            // Verificar unicidad del nombre
            if (await _brandRepository.ExistsByNameAsync(sanitizedName))
            {
                throw new ConflictException($"Ya existe una marca con el nombre '{sanitizedName}'.");
            }

            // Generar slug único
            var slug = await _brandRepository.GenerateUniqueSlugAsync(sanitizedName);

            var brand = new Brand
            {
                Name = sanitizedName,
                Slug = slug,
                Description = sanitizedDescription,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdBrand = await _brandRepository.CreateAsync(brand);
            Log.Information("Marca creada: {@Brand}", createdBrand);

            var dto = createdBrand.Adapt<BrandDetailDTO>();
            dto.ProductCount = 0;

            return dto;
        }

        public async Task<BrandDetailDTO> UpdateAsync(int id, BrandUpdateDTO updateDto)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new NotFoundException($"Marca con ID {id} no encontrada.");
            }

            // Sanitizar datos de entrada
            var sanitizedName = SanitizeInput(updateDto.Name);
            var sanitizedDescription = !string.IsNullOrEmpty(updateDto.Description)
                ? SanitizeInput(updateDto.Description)
                : null;

            // Verificar unicidad del nombre (excluyendo la marca actual)
            if (await _brandRepository.ExistsByNameAsync(sanitizedName, id))
            {
                throw new ConflictException($"Ya existe otra marca con el nombre '{sanitizedName}'.");
            }

            // Generar nuevo slug si el nombre cambió
            string newSlug = brand.Slug;
            if (brand.Name != sanitizedName)
            {
                newSlug = await _brandRepository.GenerateUniqueSlugAsync(sanitizedName, id);
            }

            brand.Name = sanitizedName;
            brand.Slug = newSlug;
            brand.Description = sanitizedDescription;

            var updatedBrand = await _brandRepository.UpdateAsync(brand);
            var productCount = await _brandRepository.CountProductsByBrand(id);

            var dto = updatedBrand.Adapt<BrandDetailDTO>();
            dto.ProductCount = productCount;

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new NotFoundException($"Marca con ID {id} no encontrada.");
            }

            // Verificar si hay productos asociados
            var productCount = await _brandRepository.CountProductsByBrand(id);
            if (productCount > 0)
            {
                throw new ConflictException(
                    $"No se puede eliminar la marca '{brand.Name}' porque tiene {productCount} producto(s) asociado(s). " +
                    "Elimine o reasigne los productos antes de eliminar la marca."
                );
            }

            return await _brandRepository.DeleteAsync(id);
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