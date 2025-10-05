using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TiendaProyecto.src.api.Controllers;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Exceptions;

namespace TiendaProyecto.src.API.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los productos.
    /// </summary>
    public class ProductController : BaseController
    {
        /// <summary>
        /// Controlador para manejar las operaciones relacionadas con los productos.
        /// </summary>
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene todos los productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        [HttpGet("admin/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllForAdminAsync([FromQuery] SearchParamsDTO searchParams)
        {
            var totalItems = await _productService.CountFilteredForAdminAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalItems / (searchParams.PageSize ?? 10));

            if (searchParams.PageNumber > totalPages && totalPages > 0)
            {
                throw new BadRequestAppException("El número de página excede el total de páginas disponibles.");
            }
            var result = await _productService.GetFilteredForAdminAsync(searchParams);
            if (result == null || result.Products.Count == 0) { throw new KeyNotFoundException("No se encontraron productos."); }
            return Ok(new GenericResponse<ListedProductsForAdminDTO>("Productos obtenidos exitosamente", result));
        }

        [HttpGet("customer/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllForCustomerAsync([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _productService.GetFilteredForCustomerAsync(searchParams);
            if (result == null || result.Products.Count == 0) { throw new KeyNotFoundException("No se encontraron productos."); }
            return Ok(new GenericResponse<ListedProductsForCustomerDTO>("Productos obtenidos exitosamente", result));
        }

        /// <summary>
        /// Obtiene un producto específico para el cliente.
        /// </summary>
        /// <param name="id">ID del producto a obtener.</param>
        /// <returns>El producto solicitado.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdForCustomerAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null) { throw new KeyNotFoundException("Producto no encontrado."); }
            return Ok(new GenericResponse<ProductDetailDTO>("Producto obtenido exitosamente", result));
        }

        /// <summary>
        /// Obtiene un producto específico para el admin.
        /// </summary>
        /// <param name="id">ID del producto a obtener.</param>
        /// <returns>El producto solicitado.</returns>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdForAdminAsync(int id)
        {
            var result = await _productService.GetByIdForAdminAsync(id);
            if (result == null) { throw new KeyNotFoundException("Producto no encontrado."); }
            return Ok(new GenericResponse<ProductDetailDTO>("Producto obtenido exitosamente", result));
        }

        /// <summary>
        /// Crea un nuevo producto en el sistema.
        /// </summary>
        /// <param name="createProductDTO">Los datos del producto a crear.</param>
        /// <returns>El ID del producto creado.</returns>
        [HttpPost()]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductDTO createProductDTO)
        {
           if (!ModelState.IsValid)
                {
                    // Convertir errores de ModelState en tu payload estándar
                    var errors = ModelState
                        .Where(kvp => kvp.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        code = "VALIDATION_ERROR",
                        message = "One or more validation errors occurred.",
                        traceId = HttpContext.TraceIdentifier,
                        path = HttpContext.Request.Path.Value,
                        method = HttpContext.Request.Method,
                        timestamp = DateTime.UtcNow,
                        errors
                    });
                }

                var result = await _productService.CreateAsync(createProductDTO);
                return Created($"/api/product/{result}", new GenericResponse<string>("Producto creado exitosamente", result));
        }

        /// <summary>
        /// Cambia el estado activo de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
        /// <returns>Una respuesta que indica el resultado de la operación.</returns>
        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActiveAsync(int id)
        {
            await _productService.ToggleActiveAsync(id);
            return Ok(new GenericResponse<string>("Estado del producto actualizado exitosamente", "El estado del producto ha sido cambiado."));
        }
    }
}