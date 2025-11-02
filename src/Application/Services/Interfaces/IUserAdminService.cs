using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Application.DTO.BaseResponse;


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

        /// <summary>
        /// Actualiza el estado de un usuario (bloquear/desbloquear).
        /// </summary>
        /// <param name="userId">ID del usuario a modificar</param>
        /// <param name="updateDto">DTO con el nuevo estado y razón</param>
        /// <param name="adminId">ID del administrador que realiza el cambio</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDTO updateDto, int adminId);
    
        Task<GenericResponse<object>> UpdateUserRoleAsync(int userId, UpdateUserRoleDTO dto, string adminId);
    }
}