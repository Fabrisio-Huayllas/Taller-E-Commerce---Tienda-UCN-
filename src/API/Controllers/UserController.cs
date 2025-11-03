using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TiendaProyecto.src.api.Controllers;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Application.Services.Interfaces;

namespace TiendaProyecto.src.API.Controllers
{
    /// <summary>
    /// Controlador para la administración de usuarios (solo administradores).
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly IUserAdminService _userAdminService;

        public UsersController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        /// <summary>
        /// Obtiene todos los usuarios con paginación, filtros y búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda, filtros y paginación.</param>
        /// <returns>Lista paginada de usuarios.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserSearchParamsDTO searchParams)
        {
            var result = await _userAdminService.GetUsersAsync(searchParams);
            return Ok(new GenericResponse<ListedUsersForAdminDTO>("Usuarios obtenidos exitosamente", result));
        }

        /// <summary>
        /// Obtiene el detalle completo de un usuario por ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Detalle completo del usuario.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            var result = await _userAdminService.GetUserDetailAsync(id);
            return Ok(new GenericResponse<UserDetailForAdminDTO>("Detalle de usuario obtenido exitosamente", result));
        }

        /// <summary>
        /// Actualiza el estado de un usuario (bloquear/desbloquear).
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <param name="updateDto">DTO con el nuevo estado y razón.</param>
        /// <returns>Confirmación de la actualización.</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDTO updateDto)
        {
            // Obtener ID del administrador desde el token
            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                return Unauthorized(new GenericResponse<object>("Administrador no autenticado", null));
            }

            var result = await _userAdminService.UpdateUserStatusAsync(id, updateDto, adminId);

            if (result)
            {
                var message = updateDto.Status.ToLower() == "blocked"
                    ? "Usuario bloqueado exitosamente"
                    : "Usuario desbloqueado exitosamente";

                return Ok(new GenericResponse<object>(message, null));
            }

            return BadRequest(new GenericResponse<object>("No se pudo actualizar el estado del usuario", null));
        }

        /// <summary>
        /// R140: Actualiza el rol de un usuario (solo Admin)
        /// </summary>
        /// <param name="id">ID del usuario a modificar</param>
        /// <param name="dto">Datos del nuevo rol</param>
        /// <returns>Respuesta con información del cambio realizado</returns>
        [HttpPatch("{id}/role")]
        [ProducesResponseType(typeof(GenericResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO dto)
        {
            // Obtener el ID del admin desde el token JWT
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new GenericResponse<object>("No se pudo identificar al usuario autenticado"));
            }
            
            // El servicio lanzará excepciones que el ErrorHandlerMiddleware capturará
            var response = await _userAdminService.UpdateUserRoleAsync(id, dto, adminId);
            
            return Ok(response);
        }
        
    }
}