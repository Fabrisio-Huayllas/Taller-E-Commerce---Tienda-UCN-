using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de administración de usuarios.
    /// </summary>
    public interface IUserAdminService
    {
        /// <summary>
        /// Obtiene usuarios con filtros y paginación para administradores.
        /// </summary>
        Task<ListedUsersForAdminDTO> GetUsersAsync(UserSearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene el detalle completo de un usuario para administradores.
        /// </summary>
        Task<UserDetailForAdminDTO> GetUserDetailAsync(int id);
    }
}