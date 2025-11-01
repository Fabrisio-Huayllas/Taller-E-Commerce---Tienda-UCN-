using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaProyecto.src.api.Controllers;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.DTO.OrderDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO;


namespace TiendaProyecto.src.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de órdenes (clientes y administradores).
    /// </summary>
    [ApiController]
    [Route("api/orders")]               // <- ruta cliente
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        /// <summary>
        /// Crea una nueva orden (cliente autenticado).
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

            int.TryParse(userId, out int parsedUserId);
            var result = await _orderService.CreateAsync(parsedUserId);
            return Created($"api/order/detail/{result}", new GenericResponse<string>("Orden creada exitosamente", result));
        }

        /// <summary>
        /// Obtiene los detalles de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>Detalles de la orden encontrada.</returns>
        [HttpGet("detail/{orderCode}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderDetail(string orderCode)
        {
            var result = await _orderService.GetDetailAsync(orderCode);
            return Ok(new GenericResponse<OrderDetailDTO>("Detalle de orden obtenido exitosamente", result));
        }

        /// <summary>
        /// Obtiene las órdenes de un usuario.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda</param>
        /// <returns>Órdenes del usuario.</returns>
        [HttpGet("user-orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders([FromQuery] SearchParamsDTO searchParams)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

            int.TryParse(userId, out int parsedUserId);
            var result = await _orderService.GetByUserIdAsync(searchParams, parsedUserId);
            return Ok(new GenericResponse<ListedOrderDetailDTO>("Órdenes del usuario obtenidas exitosamente", result));
        }

        //========== ADMIN ==========

        /// <summary>
        /// Obtiene una lista de todas las órdenes (solo administradores).
        /// </summary>
        [HttpGet("admin/list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _orderService.GetAllAsync(searchParams);
            return Ok(new GenericResponse<ListedOrdersForAdminDTO>("Órdenes obtenidas exitosamente", result));
        }

        /// <summary>
        /// Cambia el estado de una orden (solo administradores).
        /// </summary>
        [HttpPut("admin/{orderCode}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeOrderStatus(string orderCode, [FromBody] UpdateOrderStatusDTO request)
        {
            var adminId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Administrador no autenticado.");

            int.TryParse(adminId, out int parsedAdminId);
            await _orderService.ChangeStatusAsync(orderCode, request.Status, parsedAdminId, request.Note);
            return NoContent(); //
        }
    }
}