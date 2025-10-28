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
        private readonly IImageService _imageService;

        private readonly int _defaultPageSize;

        public ProductService(IProductRepository productRepository, IConfiguration configuration, IFileService fileService,IImageService imageService)
        {
            _productRepository = productRepository;
            _configuration = configuration;
            _fileService = fileService;
            _imageService = imageService;
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
        
        public async Task UpdateAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Producto con ID {id} no encontrado.");
            }

            // Actualizar los campos del producto
            product.Title = updateProductDTO.Name;
            product.Description = updateProductDTO.Description;
            product.Price = updateProductDTO.Price;
            product.Stock = updateProductDTO.Stock;
            product.CategoryId = updateProductDTO.CategoryId;
            product.BrandId = updateProductDTO.BrandId;

            await _productRepository.UpdateAsync(product);
        }
        /// <summary>
        /// Elimina lógicamente un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DeleteAsync(int id)
{
    var product = await _productRepository.GetByIdForAdminAsync(id);
    if (product == null)
    {
        Log.Warning("Producto con ID {ProductId} no encontrado para eliminación", id);
        throw new NotFoundException($"Producto con ID {id} no encontrado.");
    }

    // R94: Obtener imágenes ANTES de soft delete
    var images = await _productRepository.GetProductImagesAsync(id);
    
    Log.Information("Producto {ProductId} tiene {ImageCount} imágenes para eliminar", id, images.Count);
    
    // 1. PRIMERO: Eliminar imágenes de Cloudinary y BD
    if (images.Count > 0)
    {
        foreach (var image in images)
        {
            try
            {
                // Eliminar de Cloudinary
                await _imageService.DeleteImageAsync(image.PublicId);
                Log.Information("Imagen {ImageId} ({PublicId}) eliminada de Cloudinary", 
                    image.Id, image.PublicId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar imagen {ImageId} de Cloudinary", image.Id);
            }
        }
        
        // Eliminar físicamente TODAS las imágenes de la BD
        foreach (var image in images)
        {
            await _productRepository.DeleteImageAsync(image.Id);
        }
        
        Log.Information("Todas las {Count} imágenes del producto {ProductId} eliminadas de BD", 
            images.Count, id);
    }

    // 2. DESPUÉS: Soft delete del producto
    await _productRepository.SoftDeleteAsync(id);
    
    Log.Information("Producto con ID {ProductId} eliminado lógicamente. Total de imágenes eliminadas: {ImageCount}", 
        id, images.Count);
}

        /// <summary>
        /// Agrega imágenes a un producto (R91, R92).
        /// </summary>
        public async Task<List<Image>> AddImagesAsync(int productId, List<IFormFile> files)
        {
            // Validar que el producto existe
            var product = await _productRepository.GetByIdForAdminAsync(productId);
            if (product == null)
            {
                Log.Warning("Producto con ID {ProductId} no encontrado para agregar imágenes", productId);
                throw new NotFoundException($"Producto con ID {productId} no encontrado");
            }

            // Validar que se proporcionaron archivos
            if (files == null || files.Count == 0)
            {
                throw new BadRequestAppException("No se proporcionaron archivos de imagen");
            }

            // Validar límite de imágenes (opcional, ajusta según necesites)
            const int maxImages = 10;
            var currentImagesCount = await _productRepository.GetProductImagesAsync(productId);
            if (currentImagesCount.Count + files.Count > maxImages)
            {
                throw new BadRequestAppException($"El producto no puede tener más de {maxImages} imágenes");
            }

            var images = new List<Image>();

            try
            {
                foreach (var file in files)
                {
                    // Subir imagen a Cloudinary
                    var (url, publicId) = await _imageService.UploadImageAsync(file);

                    // Crear registro de imagen
                    var image = new Image
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        ProductId = productId,
                        CreatedAt = DateTime.UtcNow
                    };

                    images.Add(image);
                }

                // Guardar en la base de datos
                await _productRepository.AddImagesAsync(images);

                Log.Information("{Count} imágenes agregadas exitosamente al producto {ProductId}", 
                    images.Count, productId);

                return images;
            }
            catch (Exception ex)
            {
                // Si algo falla, intentar eliminar las imágenes que se subieron a Cloudinary
                Log.Error(ex, "Error al agregar imágenes al producto {ProductId}. Limpiando...", productId);
                
                foreach (var image in images.Where(i => !string.IsNullOrEmpty(i.PublicId)))
                {
                    try
                    {
                        await _imageService.DeleteImageAsync(image.PublicId);
                    }
                    catch (Exception cleanupEx)
                    {
                        Log.Error(cleanupEx, "Error al limpiar imagen {PublicId}", image.PublicId);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Elimina una imagen de un producto (R93, R94).
        /// </summary>
        public async Task DeleteImageAsync(int productId, int imageId)
        {
            // Obtener la imagen
            var image = await _productRepository.GetImageByIdAsync(imageId);

            // Validar que existe y pertenece al producto
            if (image == null || image.ProductId != productId)
            {
                Log.Warning("Imagen {ImageId} no encontrada o no pertenece al producto {ProductId}", 
                    imageId, productId);
                throw new NotFoundException("Imagen no encontrada");
            }

            try
            {
                // Eliminar de Cloudinary primero
                await _imageService.DeleteImageAsync(image.PublicId);

                // Eliminar de la base de datos
                await _productRepository.DeleteImageAsync(imageId);

                Log.Information("Imagen {ImageId} eliminada exitosamente del producto {ProductId}", 
                    imageId, productId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar imagen {ImageId} del producto {ProductId}", 
                    imageId, productId);
                throw;
            }
        }



    }
}