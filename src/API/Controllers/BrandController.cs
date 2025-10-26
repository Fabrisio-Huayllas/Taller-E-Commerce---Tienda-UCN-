using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaProyecto.src.api.Controllers;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.DTO.BrandDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;

namespace TiendaProyecto.src.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de marcas (solo administradores).
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Obtiene todas las marcas con paginación y búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda y paginación.</param>
        /// <returns>Lista paginada de marcas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _brandService.GetAllAsync(searchParams);
            return Ok(new GenericResponse<ListedBrandsDTO>("Marcas obtenidas exitosamente", result));
        }

        /// <summary>
        /// Obtiene una marca por ID.
        /// </summary>
        /// <param name="id">ID de la marca.</param>
        /// <returns>Detalle de la marca.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetByIdAsync(id);
            return Ok(new GenericResponse<BrandDetailDTO>("Marca obtenida exitosamente", result));
        }

        /// <summary>
        /// Crea una nueva marca.
        /// </summary>
        /// <param name="createDto">Datos para crear la marca.</param>
        /// <returns>Marca creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandCreateDTO createDto)
        {
            var result = await _brandService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                new GenericResponse<BrandDetailDTO>("Marca creada exitosamente", result));
        }

        /// <summary>
        /// Actualiza una marca existente.
        /// </summary>
        /// <param name="id">ID de la marca a actualizar.</param>
        /// <param name="updateDto">Datos actualizados de la marca.</param>
        /// <returns>Marca actualizada.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDTO updateDto)
        {
            var result = await _brandService.UpdateAsync(id, updateDto);
            return Ok(new GenericResponse<BrandDetailDTO>("Marca actualizada exitosamente", result));
        }

        /// <summary>
        /// Elimina una marca.
        /// </summary>
        /// <param name="id">ID de la marca a eliminar.</param>
        /// <returns>Confirmación de eliminación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteAsync(id);
            if (result)
            {
                return Ok(new GenericResponse<object>("Marca eliminada exitosamente", null));
            }
            else
            {
                return BadRequest(new GenericResponse<object>("No se pudo eliminar la marca", null));
            }
        }
    }
}