using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Verifica si un código de orden existe.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>True si el código existe, de lo contrario false.</returns>
        Task<bool> CodeExistsAsync(string orderCode);

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        /// <param name="order">La orden a crear.</param>
        /// <returns>Booleano que indica si la creación fue exitosa.</returns>
        Task<bool> CreateAsync(Order order);

        /// <summary>
        /// Obtiene una orden por su código.
        /// </summary>
        /// <param name="orderCode">El código de la orden.</param>
        /// <returns>La orden correspondiente o null si no se encuentra.</returns>
        Task<Order?> GetByCodeAsync(string orderCode);

        /// <summary>
        /// Obtiene las órdenes de un usuario por su ID con filtros y paginación.
        /// </summary>
        /// <param name="searchParams">Los parámetros de búsqueda.</param>
        /// <param name="userId">El ID del usuario.</param>
        /// <returns>Una lista de órdenes y el total de registros.</returns>
        Task<(List<Order> Orders, int TotalCount)> GetByUserIdAsync(SearchParamsDTO searchParams, int userId);

        /// <summary>
        /// Obtiene todas las órdenes del sistema con filtros, búsqueda y paginación (para administrador).
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda.</param>
        /// <returns>Una lista de órdenes y el total de registros.</returns>
        Task<(List<Order> Orders, int TotalCount)> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        /// <param name="order">La orden a actualizar.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        Task<bool> UpdateAsync(Order order);
    }
}