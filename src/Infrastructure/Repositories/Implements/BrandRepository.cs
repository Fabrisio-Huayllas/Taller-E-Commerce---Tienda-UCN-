using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de marcas.
    /// </summary>
    public class BrandRepository : IBrandRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public BrandRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<(IEnumerable<Brand> brands, int totalCount)> GetAllAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Brands
                .Where(b => b.IsActive)
                .AsNoTracking();

            // Búsqueda por término
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.Name.ToLower().Contains(searchTerm) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            // Paginación
            var pageNumber = searchParams.PageNumber ?? 1;
            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            var brands = await query
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (brands, totalCount);
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands
                .Include(b => b.Products.Where(p => p.IsAvailable))
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<Brand?> GetBySlugAsync(string slug)
        {
            return await _context.Brands
                .Include(b => b.Products.Where(p => p.IsAvailable))
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsActive);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Brands.Where(b => b.Name.ToLower() == name.ToLower() && b.IsActive);

            if (excludeId.HasValue)
                query = query.Where(b => b.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null)
        {
            var query = _context.Brands.Where(b => b.Slug == slug && b.IsActive);

            if (excludeId.HasValue)
                query = query.Where(b => b.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Brand> CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<Brand> UpdateAsync(Brand brand)
        {
            brand.UpdatedAt = DateTime.UtcNow;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            brand.IsActive = false;
            brand.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountProductsByBrand(int brandId)
        {
            return await _context.Products
                .Where(p => p.BrandId == brandId && p.IsAvailable)
                .CountAsync();
        }

        public async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null)
        {
            var baseSlug = NormalizeSlug(name);
            var slug = baseSlug;
            var counter = 1;

            while (await ExistsBySlugAsync(slug, excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        private static string NormalizeSlug(string input)
        {
            // Convertir a minúsculas
            var slug = input.ToLowerInvariant();

            // Remover acentos
            slug = RemoveAccents(slug);

            // Reemplazar espacios y caracteres especiales con guiones
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            return slug;
        }

        private static string RemoveAccents(string text)
        {
            // Normalizar el texto para separar caracteres base de los acentos
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                // Solo mantener caracteres que NO sean marcas diacríticas (acentos)
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Normalizar de vuelta
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}