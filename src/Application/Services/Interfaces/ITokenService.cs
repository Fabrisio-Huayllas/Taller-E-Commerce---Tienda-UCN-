using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Models;   

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token JWT para el usuario proporcionado.
        /// </summary>
        /// <param name="user">El usuario para el cual se generará el token.</param>
        /// <param name="rememberMe">Indica si se debe recordar al usuario.</param>
        /// <param name="roleName">El nombre del rol del usuario.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        string GenerateToken(User user, string roleName, bool rememberMe);
    }
}