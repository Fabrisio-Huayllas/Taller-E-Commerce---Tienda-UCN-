using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica si un usuario existe por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>True si el usuario existe, false en caso contrario</returns>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Verifica si un usuario existe por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <returns>True si el usuario existe, false en caso contrario</returns>
        Task<bool> ExistsByRutAsync(string rut);

        /// <summary>
        /// Obtiene un usuario por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <param name="trackChanges">Indica si se debe rastrear los cambios en la entidad</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByRutAsync(string rut, bool trackChanges = false);

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Usuario a crear</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>True si se creó correctamente, false en caso contrario</returns>
        Task<bool> CreateAsync(User user, string password);

        /// <summary>
        /// Verifica si la contraseña proporcionada es correcta para el usuario.
        /// </summary>
        /// <param name="user">Usuario a verificar</param>
        /// <param name="password">Contraseña a verificar</param>
        /// <returns>True si la contraseña es correcta, false en caso contrario</returns>
        Task<bool> CheckPasswordAsync(User user, string password);

        /// <summary>
        /// Confirma el correo electrónico del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>True si se confirmó correctamente, false en caso contrario</returns>
        Task<bool> ConfirmEmailAsync(string email);

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="user">Usuario a actualizar</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
        Task<bool> UpdatePasswordAsync(User user, string newPassword);

        /// <summary>
        /// Elimina usuarios no confirmados.
        /// </summary>
        /// <returns>Número de usuarios eliminados</returns>
        Task<int> DeleteUnconfirmedAsync();

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
        Task<bool> DeleteAsync(int userId);

        // MÉTODOS PARA ADMINISTRACIÓN DE USUARIOS

        /// <summary>
        /// Obtiene el rol de un usuario por ID.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Nombre del rol o null si no existe</returns>
        Task<string?> GetUserRoleByIdAsync(int id);

        /// <summary>
        /// Obtiene el rol de un usuario.
        /// </summary>
        /// <param name="user">Usuario del cual obtener el rol</param>
        /// <returns>Nombre del rol</returns>
        Task<string> GetUserRoleAsync(User user);

        /// <summary>
        /// Obtiene usuarios con filtros, paginación y búsqueda para administradores.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda y filtros</param>
        /// <returns>Lista de usuarios y total de registros</returns>
        Task<(IEnumerable<User> users, int totalCount)> GetUsersForAdminAsync(UserSearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene un usuario por ID con información completa para administradores.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetUserForAdminAsync(int id);
    }
}