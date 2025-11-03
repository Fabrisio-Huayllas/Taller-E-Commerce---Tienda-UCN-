using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO
{
    /// <summary>
    /// DTO que representa una lista paginada de órdenes para el panel de administración.
    /// </summary>
    public class ListedOrdersForAdminDTO
    {
        /// <summary>
        /// Lista de órdenes en la página actual.
        /// </summary>
        public List<OrderForAdminDTO> Orders { get; set; } = new List<OrderForAdminDTO>();

        /// <summary>
        /// Cantidad total de órdenes.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Número total de páginas disponibles.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Página actual.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Tamaño de la página.
        /// </summary>
        public int PageSize { get; set; }
    }
}