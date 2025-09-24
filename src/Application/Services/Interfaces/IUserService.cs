using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.AuthDTO;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de usuarios.
    /// </summary>
    /// <summary>
    /// Interfaz para el servicio de usuarios.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el token JWT generadon y la id del usuario.</returns>
        Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext);

        Task ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);

        
        /// <summary>
        /// Elimina usuarios no confirmados.
        /// </summary>
        /// <returns>Número de usuarios eliminados</returns>
        Task<int> DeleteUnconfirmedAsync();
    }
}
