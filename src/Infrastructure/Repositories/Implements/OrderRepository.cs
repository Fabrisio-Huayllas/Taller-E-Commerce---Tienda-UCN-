using Microsoft.EntityFrameworkCore;
using TiendaProyecto.src.Application.DTO.ProductDTO;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Infrastructure.Repositories.Implements
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public OrderRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            // Inicializar page size por defecto desde configuración
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        /// <summary>
        /// Verifica si un código de orden existe.
        /// </summary>
        public async Task<bool> CodeExistsAsync(string orderCode)
        {
            return await _context.Orders.AnyAsync(o => o.Code == orderCode);
        }

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        public async Task<bool> CreateAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene una orden por su código.
        /// </summary>
        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            return await _context.Orders.Include(or => or.OrderItems).FirstOrDefaultAsync(o => o.Code == orderCode);
        }

        /// <summary>
        /// Obtiene las órdenes de un usuario por su ID con filtros y paginación.
        /// </summary>
        public async Task<(List<Order> Orders, int TotalCount)> GetByUserIdAsync(SearchParamsDTO searchParams, int userId)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            // Filtrado por número de orden
            if (!string.IsNullOrWhiteSpace(searchParams.OrderNumber))
            {
                var num = searchParams.OrderNumber.Trim().ToLower();
                query = query.Where(o => o.Code.ToLower().Contains(num));
            }

            // Filtrado por rango de fechas
            if (searchParams.From.HasValue)
                query = query.Where(o => o.CreatedAt >= searchParams.From.Value);
            if (searchParams.To.HasValue)
                query = query.Where(o => o.CreatedAt <= searchParams.To.Value);

            // Filtrado por status
            if (searchParams.Status.HasValue)
                query = query.Where(o => o.Status == searchParams.Status.Value);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
                {
                    var term = searchParams.SearchTerm.Trim().ToLower();
                    query = query.Where(o =>
                        o.Code.ToLower().Contains(term) ||
                        (o.User != null && o.User.Email != null && (
                            o.User.Email.ToLower().Contains(term) ||
                            o.User.FirstName.ToLower().Contains(term) ||
                            o.User.LastName.ToLower().Contains(term) ||
                            o.User.Rut.ToLower().Contains(term)
                        ))
                    );
                }
            var totalCount = await query.CountAsync();

            // Normalizar pageNumber y limitar pageSize al máximo configurado
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            int pageNumber = Math.Max(1, searchParams.PageNumber ?? 1);

            // Orden por createdAt desc por defecto
            if (!string.IsNullOrWhiteSpace(searchParams.OrderBy))
            {
                var ob = searchParams.OrderBy!.Trim().ToLower();
                var dir = (searchParams.OrderDir ?? "desc").Trim().ToLower();
                if (ob == "total")
                {
                    query = dir == "asc" ? query.OrderBy(o => o.Total) : query.OrderByDescending(o => o.Total);
                }
                else // createdAt u otros => createdAt
                {
                    query = dir == "asc" ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var list = await query.ToListAsync();
            return (Orders: list, TotalCount: totalCount);
        }

        /// <summary>
        /// Obtiene todas las órdenes del sistema con filtros, búsqueda y paginación (para administrador).
        /// </summary>
        public async Task<(List<Order> Orders, int TotalCount)> GetAllAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Filtros combinables
            if (searchParams.Status.HasValue)
            {
                query = query.Where(o => o.Status == searchParams.Status.Value);
            }

            if (searchParams.From.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= searchParams.From.Value);
            }

            if (searchParams.To.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= searchParams.To.Value);
            }

            if (searchParams.CustomerId.HasValue)
            {
                query = query.Where(o => o.UserId == searchParams.CustomerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.CustomerEmail))
            {
                var email = searchParams.CustomerEmail.Trim().ToLower();
                query = query.Where(o => o.User != null && o.User.Email != null && o.User.Email.ToLower().Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.OrderNumber))
            {
                var num = searchParams.OrderNumber.Trim().ToLower();
                query = query.Where(o => o.Code.ToLower().Contains(num));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var term = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(term) ||
                    (o.User != null && o.User.Email != null && (
                        o.User.Email.ToLower().Contains(term) ||
                        o.User.FirstName.ToLower().Contains(term) ||
                        o.User.LastName.ToLower().Contains(term) ||
                        o.User.Rut.ToLower().Contains(term)
                    ))
                );
            }

            var totalCount = await query.CountAsync();

            // Normalizar pageNumber y limitar pageSize al máximo configurado
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            int pageNumber = Math.Max(1, searchParams.PageNumber ?? 1);

            // Ordenamiento seguro (whitelist)
            var orderBy = (searchParams.OrderBy ?? "createdAt").Trim().ToLower();
            var orderDir = (searchParams.OrderDir ?? "desc").Trim().ToLower();

            if (orderBy == "total")
            {
                query = orderDir == "asc" ? query.OrderBy(o => o.Total) : query.OrderByDescending(o => o.Total);
            }
            else // createdAt por defecto
            {
                query = orderDir == "asc" ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var list = await query.ToListAsync();
            return (Orders: list, TotalCount: totalCount);
        }

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        public async Task<bool> UpdateAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}