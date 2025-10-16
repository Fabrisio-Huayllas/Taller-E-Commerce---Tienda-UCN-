using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Application.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        private readonly int _defaultPageSize;

        public ProductService(IProductRepository productRepository, IConfiguration configuration, IFileService fileService)
        {
            _productRepository = productRepository;
            _configuration = configuration;
            _fileService = fileService;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? throw new InvalidOperationException("La configuración 'DefaultPageSize' no está definida."));
        }

        /// <summary>
        /// Crea un nuevo producto en el sistema.
        /// </summary>
        /// <param name="createProductDTO">Los datos del producto a crear.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el ID del producto creado.</returns>
        public async Task<string> CreateAsync(CreateProductDTO createProductDTO)
        {
            // Validar los datos del producto
    if (string.IsNullOrWhiteSpace(createProductDTO.Title))
    {
        throw new BadRequestAppException("El nombre del producto es obligatorio.");
    }

    if (createProductDTO.Price <= 0)
    {
        throw new BadRequestAppException("El precio del producto debe ser mayor a 0.");
    }

    if (createProductDTO.Stock < 0)
    {
        throw new BadRequestAppException("El stock del producto no puede ser negativo.");
    }

    if (createProductDTO.CategoryName == null)
    {
        throw new BadRequestAppException("La categoría del producto es obligatoria.");
    }

    if (createProductDTO.Images == null || !createProductDTO.Images.Any())
    {
        throw new BadRequestAppException("Debe proporcionar al menos una imagen para el producto.");
    }

            Product product = createProductDTO.Adapt<Product>();
            Category category = await _productRepository.CreateOrGetCategoryAsync(createProductDTO.CategoryName) ?? throw new Exception("Error al crear o obtener la categoría del producto.");
            Brand brand = await _productRepository.CreateOrGetBrandAsync(createProductDTO.BrandName) ?? throw new Exception("Error al crear o obtener la marca del producto.");
            product.CategoryId = category.Id;
            product.BrandId = brand.Id;
            product.Images = new List<Image>();
            int productId = await _productRepository.CreateAsync(product);
            Log.Information("Producto creado: {@Product}", product);
            if (createProductDTO.Images == null || !createProductDTO.Images.Any())
            {
                Log.Information("No se proporcionaron imágenes. Se asignará la imagen por defecto.");
                throw new InvalidOperationException("Debe proporcionar al menos una imagen para el producto.");
            }
            foreach (var image in createProductDTO.Images)
            {
                Log.Information("Imagen asociada al producto: {@Image}", image);
                await _fileService.UploadAsync(image, productId);
            }
            return product.Id.ToString();
        }

        /// <summary>
        /// Retorna un producto específico por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<ProductDetailDTO> GetByIdAsync(int id)
        {
            // Consulta optimizada para cargar solo lo necesario
            var product = await _productRepository.Query()
                .Where(p => p.Id == id && p.IsAvailable) // Solo productos activos
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Si el producto no existe o no está activo, lanzar excepción 404
            if (product == null)
            {
                throw new NotFoundException($"Producto con ID {id} no encontrado o no está disponible.");
            }

            // Mapear a DTO
            return new ProductDetailDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImagesURL = product.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                Price = product.Price.ToString("C0"), // Formato de precio como string (ejemplo: "$19,990")
                Discount = product.Discount,
                Stock = product.Stock,
                StockIndicator = GetStockIndicator(product.Stock), // Indicador de stock
                CategoryName = product.Category.Name,
                BrandName = product.Brand.Name,
                StatusName = product.IsAvailable ? "Activo" : "Inactivo",
                IsAvailable = product.IsAvailable
            };
        }
        
        /// <summary>
        /// Devuelve un indicador de stock basado en la cantidad disponible.
        /// </summary>
        /// <param name="stock">Cantidad de stock disponible.</param>
        /// <returns>Indicador de stock como cadena.</returns>
        private string GetStockIndicator(int stock)
        {
            if (stock == 0) return "Sin stock";
            if (stock <= 10) return "Bajo";
            if (stock <= 50) return "Medio";
            return "Alto";
        }

        /// <summary>
        /// Retorna un producto específico por su ID desde el punto de vista de un admin.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<ProductDetailDTO> GetByIdForAdminAsync(int id)
        {
            var product = await _productRepository.GetByIdForAdminAsync(id) ?? throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
            Log.Information("Producto encontrado: {@Product}", product);
            return product.Adapt<ProductDetailDTO>();
        }

        /// <summary>
        /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        public async Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(SearchParamsDTO searchParams)
        {
            Log.Information("Obteniendo productos para administrador con parámetros de búsqueda: {@SearchParams}", searchParams);
            var (products, totalCount) = await _productRepository.GetFilteredForAdminAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
            int currentPage = searchParams.PageNumber;
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (currentPage < 1 || currentPage > totalPages)
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }
            Log.Information("Total de productos encontrados: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}", totalCount, totalPages, currentPage, pageSize);

            // Convertimos los productos filtrados a DTOs para la respuesta
            return new ListedProductsForAdminDTO
            {
                Products = products.Adapt<List<ProductForAdminDTO>>(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

        }

        /// <summary>
        /// Retorna todos los productos para el cliente según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el cliente.</returns>
        public async Task<ListedProductsForCustomerDTO> GetFilteredForCustomerAsync(SearchParamsDTO searchParams)
        {

            // Validaciones de entrada
            if (searchParams.PageNumber < 1)
            {
                throw new BadRequestAppException("El número de página debe ser mayor o igual a 1.");
            }

            if (searchParams.PageSize.HasValue && searchParams.PageSize > 50)
            {
                throw new BadRequestAppException("El tamaño de página no puede ser mayor a 50.");
            }

            if (searchParams.MinPrice.HasValue && searchParams.MaxPrice.HasValue && searchParams.MinPrice > searchParams.MaxPrice)
            {
                throw new BadRequestAppException("El precio mínimo no puede ser mayor que el precio máximo.");
            }

            // Construcción de la consulta
            var query = _productRepository.Query()
                .Where(p => p.IsAvailable) // Solo productos activos
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1)) // Carga solo la imagen principal
                .AsNoTracking();

            // Filtros
            if (searchParams.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchParams.CategoryId);
            }

            if (searchParams.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == searchParams.BrandId);
            }

            if (searchParams.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchParams.MinPrice);
            }

            if (searchParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchParams.MaxPrice);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(searchTerm) || p.Description.ToLower().Contains(searchTerm));
            }

            // Ordenamiento
            if (!string.IsNullOrWhiteSpace(searchParams.SortBy))
            {
                var allowedSortFields = new[] { "price", "createdAt", "title" };
                if (!allowedSortFields.Contains(searchParams.SortBy.ToLower()))
                {
                    throw new BadRequestAppException("Campo de ordenamiento no permitido.");
                }

                query = searchParams.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => EF.Property<object>(p, searchParams.SortBy))
                    : query.OrderBy(p => EF.Property<object>(p, searchParams.SortBy));
            }

            // Total de productos
            var totalCount = await query.CountAsync();

            // Paginación
            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            var currentPage = searchParams.PageNumber;
            var products = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convertimos los productos filtrados a DTOs para la respuesta
            return new ListedProductsForCustomerDTO
            {
                Products = products.Adapt<List<ProductForCustomerDTO>>(),
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = currentPage,
                PageSize = pageSize
                };
                }

        /// <summary>
        /// Cambia el estado activo de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
        public async Task ToggleActiveAsync(int id)
        {
            await _productRepository.ToggleActiveAsync(id);
        }

        public async Task<int> CountFilteredForAdminAsync(SearchParamsDTO searchParams)
        {
            // Aquí puedes usar tu repositorio para contar los productos filtrados
            return await _productRepository.CountFilteredAsync(searchParams);
        }
        

    }
}