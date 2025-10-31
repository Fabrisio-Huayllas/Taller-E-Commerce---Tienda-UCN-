using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TiendaProyecto.src.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET api/admin/orders
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _orderService.GetAllAsync(searchParams);
            return Ok(result);
        }

        // GET api/admin/orders/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> GetDetail(string code)
        {
            var dto = await _orderService.GetDetailAsync(code);
            return Ok(dto);
        }

        // PATCH api/admin/orders/{code}/status
        [HttpPatch("{code}/status")]
        public async Task<IActionResult> ChangeStatus(string code, [FromBody] UpdateOrderStatusDTO req)
        {
            if (req == null) return BadRequest();

            int? adminId = null;
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "id" || c.Type == "sub");
            if (idClaim != null && int.TryParse(idClaim.Value, out var parsed)) adminId = parsed;

            await _orderService.ChangeStatusAsync(code, req.Status, adminId.GetValueOrDefault(), req.Note);
            return NoContent(); // 204 on success
        }
    }
}