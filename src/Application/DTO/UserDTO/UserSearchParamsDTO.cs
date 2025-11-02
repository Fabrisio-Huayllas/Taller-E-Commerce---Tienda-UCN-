using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.ProductDTO;

namespace TiendaProyecto.src.Application.DTO.UserDTO
{
    /// <summary>
    /// DTO que extiende SearchParamsDTO para búsquedas específicas de usuarios.
    /// </summary>
    public class UserSearchParamsDTO : SearchParamsDTO
    {
        /// <summary>
        /// Filtro por rol (Admin, Customer).
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Filtro por estado (Active, Blocked).
        /// </summary>
        public new string? Status { get; set; }

        /// <summary>
        /// Búsqueda parcial por email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Fecha desde para filtrar por creación.
        /// </summary>
        public DateTime? CreatedFrom { get; set; }

        /// <summary>
        /// Fecha hasta para filtrar por creación.
        /// </summary>
        public DateTime? CreatedTo { get; set; }
    }
}