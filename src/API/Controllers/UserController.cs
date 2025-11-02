using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}