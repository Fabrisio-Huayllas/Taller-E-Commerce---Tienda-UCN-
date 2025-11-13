using Mapster;
using Serilog;
using TiendaProyecto.src.Application.DTO.OrderDTO;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Enums;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using TiendaProyecto.src.Application.DTO.CartDTO;
using TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaProyecto.src.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TiendaProyecto.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TiendaProyecto.src.Application.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly DataContext _context;
        private readonly int _defaultPageSize;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IConfiguration configuration,
            DataContext context)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _configuration = configuration;
            _context = context;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }
 
         // ========= CLIENTE =========
 
         /// <summary>
         /// Crea una nueva orden y vacía el carrito de compras.
         /// </summary>
        public async Task<string> CreateAsync(int userId)
        {
            // Para garantizar atomicidad usamos el mismo DataContext y una transacción.
            // Cargamos el carrito, sus items y productos en el mismo DbContext
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId)
                ?? throw new NotFoundException("Carrito no encontrado");
 
            if (cart.CartItems == null || cart.CartItems.Count == 0)
                throw new BadRequestAppException("El carrito está vacío. No se puede crear una orden.");
 
            string orderCode = await GenerateOrderCodeAsync();
 
            var newOrder = new Order
            {
                Code = orderCode,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = OrderStatus.Created,
                StatusChangedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>(),
                SubTotal = 0,
                Total = 0
            };
 
            int subTotal = 0;
            int total = 0;
 
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var ci in cart.CartItems.ToList())
                {
                    // Producto ya cargado por el Include; si no, lo buscamos desde este mismo contexto.
                    var product = ci.Product ?? await _context.Products.FirstOrDefaultAsync(p => p.Id == ci.ProductId)
                                  ?? throw new NotFoundException($"Producto {ci.ProductId} no encontrado.");
 
                    if (product.Stock < ci.Quantity)
                        throw new BadRequestAppException($"Stock insuficiente para {product.Title}. Disponible: {product.Stock}, pedido: {ci.Quantity}.");
 
                    int priceAtMoment = product.Price;
                    int discountAtMoment = product.Discount;
                    int discountedPrice = priceAtMoment - (priceAtMoment * discountAtMoment / 100);
 
                    var oi = new OrderItem
                    {
                        TitleAtMoment = product.Title ?? string.Empty,
                        DescriptionAtMoment = product.Description ?? string.Empty,
                        ImageAtMoment = product.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty,
                        Quantity = ci.Quantity,
                        PriceAtMoment = discountedPrice,
                        DiscountAtMoment = discountAtMoment
                    };
 
                    newOrder.OrderItems.Add(oi);
 
                    int line = discountedPrice * ci.Quantity;
                    subTotal += line;
                    total += line;
 
                    // Descontar stock en el mismo DbContext
                    product.Stock -= ci.Quantity;
                    _context.Products.Update(product);
                }
 
                newOrder.SubTotal = subTotal;
                newOrder.Total = total;
 
                // Persistir la orden y los cambios (productos, carrito) en la misma transacción/contexto
                _context.Orders.Add(newOrder);
 
                // Eliminar items del carrito (persistente)
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.SubTotal = 0;
                cart.Total = 0;
                cart.UpdatedAt = DateTime.UtcNow;
                _context.Carts.Update(cart);
 
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
 
                return orderCode;
            }
            catch
            {
                try { await tx.RollbackAsync(); } catch { /* noop */ }
                throw;
            }
         }
 
        /// <summary>
        /// Obtiene los detalles de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>El detalle de la orden</returns>
        public async Task<OrderDetailDTO> GetDetailAsync(string orderCode)
        {
            Order? order = await _orderRepository.GetByCodeAsync(orderCode) ?? throw new NotFoundException("Orden no encontrada");
            return order.Adapt<OrderDetailDTO>();
        }

        /// <summary>
        /// Obtiene una lista de órdenes para un usuario específico.
        /// </summary>
        public async Task<ListedOrderDetailDTO> GetByUserIdAsync(SearchParamsDTO searchParams, int userId)
        {
            var pageNumber = searchParams.PageNumber ?? 1;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            int maxPageSize = 100;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            // Asignar valores validados
            searchParams.PageNumber = pageNumber;
            searchParams.PageSize = pageSize;

            // delegar a orderrepository
            var (orders, totalCount) = await _orderRepository.GetByUserIdAsync(searchParams, userId);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var currentPage = pageNumber > totalPages && totalPages > 0 ? totalPages : pageNumber;

            var dto = new ListedOrderDetailDTO
            {
                Orders = orders.Adapt<List<OrderDetailDTO>>(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

            return dto;
        }

        /// <summary>
        /// Genera un código único para la orden.
        /// </summary>
        private async Task<string> GenerateOrderCodeAsync()
        {
            string code;
            do
            {
                var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
                var random = Random.Shared.Next(100, 999);
                code = $"ORD-{timestamp}-{random}";
            }
            while (await _orderRepository.CodeExistsAsync(code));
            return code;
        }

        // ========= ADMIN =========

        /// <summary>
        /// Obtiene una lista paginada de todas las órdenes (para administrador).
        /// </summary>
        public async Task<ListedOrdersForAdminDTO> GetAllAsync(SearchParamsDTO searchParams)
        {
            var pageNumber = searchParams.PageNumber ?? 1;
            if (pageNumber <= 0) pageNumber = 1;

            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            int maxPageSize = 100;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            // Asignar valores validados
            searchParams.PageNumber = pageNumber;
            searchParams.PageSize = pageSize;

            // Ordenamiento seguro: whitelist
            var allowedOrderFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "createdAt", "total" };
            if (!string.IsNullOrWhiteSpace(searchParams.OrderBy) && !allowedOrderFields.Contains(searchParams.OrderBy))
            {
                throw new BadRequestAppException("Campo de ordenamiento no permitido.");
            }

            // El repositorio debe implementar filtros combinables (status, from/to, customer, orderNumber) y devolver (items, totalCount)
            var (orders, totalCount) = await _orderRepository.GetAllAsync(searchParams);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var currentPage = pageNumber > totalPages && totalPages > 0 ? totalPages : pageNumber;

            var dto = new ListedOrdersForAdminDTO
            {
                Orders = orders.Adapt<List<OrderForAdminDTO>>(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

            return dto;
        }

        /// <summary>
        /// Cambia el estado de una orden.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(string orderCode, OrderStatus newStatus, int adminId, string? reason)
        {
            var order = await _orderRepository.GetByCodeAsync(orderCode) ?? throw new NotFoundException("Orden no encontrada");

            // Idempotencia
            if (order.Status == newStatus) return true;

            // Validar transición (máquina de estados)
            if (!IsValidTransition(order.Status, newStatus))
            {
                throw new ConflictException($"Transición inválida: {order.Status} -> {newStatus}");
            }

            // Registro log básico de auditoría 
            var previous = order.Status;
            var logEntry = $"[{DateTime.UtcNow:O}] {previous} -> {newStatus} by admin:{adminId}" + (string.IsNullOrWhiteSpace(reason) ? "" : $" reason: {reason}");
            order.ChangeReason = string.IsNullOrWhiteSpace(order.ChangeReason) ? logEntry : $"{order.ChangeReason}\n{logEntry}";

            order.Status = newStatus;
            order.StatusChangedAt = DateTime.UtcNow;
            order.ChangedByAdminId = adminId;
            order.UpdatedAt = DateTime.UtcNow;

        

            return await _orderRepository.UpdateAsync(order);
        }

        // Validador simple de transiciones 
        private bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            // Matriz de transiciones permitidas:
            // Created -> Paid, Cancelled
            // Paid -> Shipped, Cancelled, Refunded
            // Shipped -> Delivered, Returned
            // Delivered -> Returned
            // Cancelled/Returned/Refunded no tienen transiciones salientes
            return (current, next) switch
            {
                (OrderStatus.Created, OrderStatus.Paid) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,

                (OrderStatus.Paid, OrderStatus.Shipped) => true,
                (OrderStatus.Paid, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Refunded) => true,

                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.Shipped, OrderStatus.Returned) => true,

                (OrderStatus.Delivered, OrderStatus.Returned) => true,

               
                _ => false
            };
        }
    }
}