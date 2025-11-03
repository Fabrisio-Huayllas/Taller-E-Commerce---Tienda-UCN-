using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.api.Controllers;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.DTO.CategoryDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;

namespace TiendaProyecto.src.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de categorías (solo administradores).
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Obtiene todas las categorías con paginación y búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda y paginación.</param>
        /// <returns>Lista paginada de categorías.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _categoryService.GetAllAsync(searchParams);
            return Ok(new GenericResponse<ListedCategoriesDTO>("Categorías obtenidas exitosamente", result));
        }

        /// <summary>
        /// Obtiene una categoría por ID.
        /// </summary>
        /// <param name="id">ID de la categoría.</param>
        /// <returns>Detalle de la categoría.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(new GenericResponse<CategoryDetailDTO>("Categoría obtenida exitosamente", result));
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="createDto">Datos para crear la categoría.</param>
        /// <returns>Categoría creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO createDto)
        {
            var result = await _categoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                new GenericResponse<CategoryDetailDTO>("Categoría creada exitosamente", result));
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="id">ID de la categoría a actualizar.</param>
        /// <param name="updateDto">Datos actualizados de la categoría.</param>
        /// <returns>Categoría actualizada.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDTO updateDto)
        {
            var result = await _categoryService.UpdateAsync(id, updateDto);
            return Ok(new GenericResponse<CategoryDetailDTO>("Categoría actualizada exitosamente", result));
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar.</param>
        /// <returns>Confirmación de eliminación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result)
            {
                return Ok(new GenericResponse<object>("Categoría eliminada exitosamente", null));
            }
            else
            {
                return BadRequest(new GenericResponse<object>("No se pudo eliminar la categoría", null));
            }
        }
    }
}