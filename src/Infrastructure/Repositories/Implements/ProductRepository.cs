using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de productos que interactúa con la base de datos.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private readonly int _defaultPageSize;

        public ProductRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = _configuration.GetValue<int?>("Products:DefaultPageSize") ?? throw new ArgumentNullException("El tamaño de página por defecto no puede ser nulo.");
        }

        /// <summary>
        /// Crea un nuevo producto en el repositorio.
        /// </summary>
        /// <param name="product">El producto a crear.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado</returns>
        public async Task<int> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        /// <summary>
        /// Crea o obtiene una marca por su nombre.
        /// </summary>
        /// <param name="brandName">El nombre de la marca.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la marca creada o encontrada.</returns>
        public async Task<Brand> CreateOrGetBrandAsync(string brandName)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);

            if (brand != null)
            {
                return brand;
            }

            // Generar slug para la nueva marca
            var slug = GenerateSlug(brandName);

            // Verificar que el slug sea único
            var counter = 1;
            var originalSlug = slug;
            while (await _context.Brands.AnyAsync(b => b.Slug == slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            brand = new Brand
            {
                Name = brandName,
                Slug = slug
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        /// <summary>
        /// Crea o obtiene una categoría por su nombre.
        /// </summary>
        /// <param name="categoryName">El nombre de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la categoría creada o encontrada.</returns>
        public async Task<Category> CreateOrGetCategoryAsync(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category != null)
            {
                return category;
            }

            // Generar slug para la nueva categoría
            var slug = GenerateSlug(categoryName);

            // Verificar que el slug sea único
            var counter = 1;
            var originalSlug = slug;
            while (await _context.Categories.AnyAsync(c => c.Slug == slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            category = new Category
            {
                Name = categoryName,
                Slug = slug
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        /// <summary>
        /// Genera un slug normalizado a partir de un nombre.
        /// </summary>
        /// <param name="name">Nombre de la categoría</param>
        /// <returns>Slug normalizado</returns>
        private static string GenerateSlug(string name)
        {
            // Convertir a minúsculas
            var slug = name.ToLowerInvariant();

            // Reemplazar espacios y caracteres especiales con guiones
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            return slug;
        }

        /// <summary>
        /// Retorna un producto específico por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.
                                        AsNoTracking().
                                        Where(p => p.Id == id && p.IsAvailable&& !p.IsDeleted).
                                        Include(p => p.Category).
                                        Include(p => p.Brand).
                                        Include(p => p.Images)
                                        .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retorna un producto específico por su ID desde el punto de vista de un admin.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<Product?> GetByIdForAdminAsync(int id)
        {
            return await _context.Products.
                                        AsNoTracking().
                                        Where(p => p.Id == id&& !p.IsDeleted).
                                        Include(p => p.Category).
                                        Include(p => p.Brand).
                                        Include(p => p.Images)
                                        .FirstOrDefaultAsync();
        }

        // <summary>
        /// Retorna una lista de productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el administrador y el conteo total de productos.</returns>
        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products
                .Where(p => !p.IsDeleted) 
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1)) // Cargamos la URL de la imagen principal a la hora de crear el producto
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm) ||
                    p.Brand.Name.ToLower().Contains(searchTerm) ||
                    p.Status.ToString().ToLower().Contains(searchTerm) ||
                    p.Price.ToString().ToLower().Contains(searchTerm) ||
                    p.Stock.ToString().ToLower().Contains(searchTerm)
                );
            }

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToArrayAsync();

            return (products, totalCount);
        }

        /// <summary>
        /// Retorna una lista de productos para el cliente with los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el cliente y el conteo total de productos.</returns>
        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products
                .Where(p => p.IsAvailable && !p.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1))
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm) ||
                    p.Brand.Name.ToLower().Contains(searchTerm) ||
                    p.Status.ToString().ToLower().Contains(searchTerm) ||
                    p.Price.ToString().ToLower().Contains(searchTerm) ||
                    p.Stock.ToString().ToLower().Contains(searchTerm)
                );
            }

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToArrayAsync();

            return (products, totalCount);
        }

        /// <summary>
        /// Obtiene el stock real de un producto por su ID.
        /// </summary>
        /// <param name="productId">El ID del producto cuyo stock se obtendrá.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el stock real del producto.</returns>
        public async Task<int> GetRealStockAsync(int productId)
        {
            return await _context.Products.AsNoTracking().Where(p => p.Id == productId && !p.IsDeleted).Select(p => p.Stock).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cambia el estado activo de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
        public async Task ToggleActiveAsync(int id)
        {
            await _context.Products.Where(p => p.Id == id).ExecuteUpdateAsync(p => p.SetProperty(p => p.IsAvailable, p => !p.IsAvailable));
        }

        /// <summary>
        /// Actualiza el stock de un producto por su ID.
        /// </summary>
        /// <param name="productId">El ID del producto cuyo stock se actualizará.</param>
        /// <param name="stock">El nuevo stock del producto.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task UpdateStockAsync(int productId, int stock)
        {
            Product? product = await _context.Products.FindAsync(productId) ?? throw new KeyNotFoundException("Producto no encontrado");
            product.Stock = stock;
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountFilteredAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products.Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchParams.SearchTerm));
            }

            return await query.CountAsync();
        }

        public IQueryable<Product> Query()
        {
            return _context.Products.Where(p => !p.IsDeleted);
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(int id)
        {
            await _context.Products
                .Where(p => p.Id == id && !p.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.IsDeleted, true)
                    .SetProperty(p => p.DeletedAt, DateTime.UtcNow)
                    .SetProperty(p => p.IsAvailable, false));
        }
        /// <summary>
        /// Agrega imágenes a la base de datos (R92).
        /// </summary>
        public async Task AddImagesAsync(List<Image> images)
        {
            await _context.Images.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene una imagen por su ID.
        /// </summary>
        public async Task<Image?> GetImageByIdAsync(int imageId)
        {
            return await _context.Images
                .FirstOrDefaultAsync(i => i.Id == imageId);
        }

        /// <summary>
        /// Elimina una imagen de la base de datos (R93).
        /// </summary>
        public async Task DeleteImageAsync(int imageId)
        {
            await _context.Images
                .Where(i => i.Id == imageId)
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// Obtiene todas las imágenes de un producto.
        /// </summary>
        public async Task<List<Image>> GetProductImagesAsync(int productId)
        {
            return await _context.Images
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

    }
}