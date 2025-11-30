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
    /// Implementación del repositorio de categorías.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public CategoryRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<(IEnumerable<Category> categories, int totalCount)> GetAllAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Categories
                .Where(c => c.IsActive)
                .AsNoTracking();

            // Búsqueda por término
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            // Paginación
            var pageNumber = searchParams.PageNumber ?? 1;
            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            var categories = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (categories, totalCount);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsAvailable))
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsAvailable))
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower() && c.IsActive);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Slug == slug && c.IsActive);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountProductsByCategory(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsAvailable)
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