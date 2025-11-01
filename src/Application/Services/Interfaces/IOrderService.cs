using TiendaProyecto.src.Application.DTO.CartDTO;
using TiendaProyecto.src.Application.DTO.OrderDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaProyecto.src.Domain.Enums;

namespace TiendaProyecto.src.Application.Services.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Crea una nueva orden y vacía el carrito de compras.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado</param>
        /// <returns>Crea una nueva orden y vacía el carrito de compras.</returns>
        Task<string> CreateAsync(int userId);

        /// <summary>
        /// Obtiene los detalles de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>El detalle de la orden</returns>
        Task<OrderDetailDTO> GetDetailAsync(string orderCode);

        /// <summary>
        /// Obtiene una lista de órdenes para un usuario específico.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda</param>
        /// <param name="userId">Id del usuario al que pertenecen las órdenes</param>
        /// <returns>Ordenes del usuario</returns>
        Task<ListedOrderDetailDTO> GetByUserIdAsync(SearchParamsDTO searchParams, int userId);

        /// <summary>
        /// Obtiene una lista paginada de todas las órdenes (para administrador).
        /// </summary>
        Task<ListedOrdersForAdminDTO> GetAllAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Cambia el estado de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden a actualizar.</param>
        /// <param name="newStatus">El nuevo estado de la orden.</param>
        /// <param name="adminId">ID del administrador que realiza el cambio.</param>
        /// <param name="reason">Motivo del cambio de estado (opcional).</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        Task<bool> ChangeStatusAsync(string orderCode, OrderStatus newStatus, int adminId, string? reason);
    }
}