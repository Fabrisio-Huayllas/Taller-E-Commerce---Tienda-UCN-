using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.UserDTO
{
    /// <summary>
    /// DTO que representa un listado paginado de usuarios para administradores.
    /// </summary>
    public class ListedUsersForAdminDTO
    {
        /// <summary>
        /// Lista de usuarios.
        /// </summary>
        public List<UserForAdminDTO> Users { get; set; } = new List<UserForAdminDTO>();

        /// <summary>
        /// Cantidad total de usuarios.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Número total de páginas disponibles.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Página actual del listado.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Cantidad de usuarios por página.
        /// </summary>
        public int PageSize { get; set; }
    }
}