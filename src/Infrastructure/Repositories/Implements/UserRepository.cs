using System.Xml.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using TiendaProyecto.src.Application.DTO.UserDTO;


namespace TiendaProyecto.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de usuarios.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly int _daysOfDeleteUnconfirmedUsers;

        public UserRepository(DataContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _daysOfDeleteUnconfirmedUsers = configuration.GetValue<int?>("Jobs:DaysOfDeleteUnconfirmedUsers")
                ?? throw new InvalidOperationException("La configuración 'Jobs:DaysOfDeleteUnconfirmedUsers' no está definida.");
        }

        /// <summary>
        /// Verifica si la contraseña proporcionada es correcta para el usuario.
        /// </summary>
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// Confirma el correo electrónico del usuario.
        /// </summary>
        public async Task<bool> ConfirmEmailAsync(string email)
        {
            var result = await _context.Users
                .Where(u => u.Email == email)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
            return result > 0;
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        public async Task<bool> CreateAsync(User user, string password)
        {
            var userResult = await _userManager.CreateAsync(user, password);
            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                return roleResult.Succeeded;
            }
            return false;
        }

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.DeleteAsync(user!);
            return result.Succeeded;
        }

        /// <summary>
        /// Elimina usuarios no confirmados (sin códigos).
        /// </summary>
        public async Task<int> DeleteUnconfirmedAsync()
        {
            Log.Information("Iniciando eliminación de usuarios no confirmados");

            var cutoffDate = DateTime.UtcNow.AddDays(_daysOfDeleteUnconfirmedUsers);

            var unconfirmedUsers = await _context.Users
                .Where(u => !u.EmailConfirmed && u.RegisteredAt < cutoffDate)
                .ToListAsync();

            if (!unconfirmedUsers.Any())
            {
                Log.Information("No se encontraron usuarios no confirmados para eliminar");
                return 0;
            }

            _context.Users.RemoveRange(unconfirmedUsers);
            await _context.SaveChangesAsync();

            Log.Information($"Eliminados {unconfirmedUsers.Count} usuarios no confirmados");
            return unconfirmedUsers.Count;
        }

        /// <summary>
        /// Verifica si un usuario existe por su correo electrónico.
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verifica si un usuario existe por su RUT.
        /// </summary>
        public async Task<bool> ExistsByRutAsync(string rut)
        {
            return await _context.Users.AnyAsync(u => u.Rut == rut);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// Obtiene un usuario por su RUT.
        /// </summary>
        public async Task<User?> GetByRutAsync(string rut, bool trackChanges = false)
        {
            if (trackChanges)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
            }

            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
        }

        /// <summary>
        /// Obtiene el rol del usuario.
        /// </summary>
        public async Task<string> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault()!;
        }
        
        public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
        {
            // Genera un token seguro para resetear la contraseña
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Resetea la contraseña usando el token
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            // R35: Invalidar sesiones activas
            if (result.Succeeded)
            {
                // Cambiar SecurityStamp fuerza que todos los tokens JWT se invaliden
                await _userManager.UpdateSecurityStampAsync(user);
                
                Log.Information("Contraseña actualizada y sesiones invalidadas para usuario ID: {UserId}", user.Id);
            }
            else
            {
                Log.Warning("Fallo al actualizar contraseña para usuario ID: {UserId}", user.Id);
            }

            return result.Succeeded;
        }

        /// <summary>
        /// Obtiene usuarios con filtros, paginación y búsqueda para administradores.
        /// </summary>
        public async Task<(IEnumerable<User> users, int totalCount)> GetUsersForAdminAsync(UserSearchParamsDTO searchParams)
        {
            var query = _context.Users.AsNoTracking();

            // Filtro por email (búsqueda parcial)
            if (!string.IsNullOrWhiteSpace(searchParams.Email))
            {
                var emailSearch = searchParams.Email.Trim().ToLower();
                query = query.Where(u => u.Email!.ToLower().Contains(emailSearch));
            }

            // Filtro por estado (activo/bloqueado)
            if (!string.IsNullOrWhiteSpace(searchParams.Status))
            {
                if (searchParams.Status.ToLower() == "active")
                    query = query.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow);
                else if (searchParams.Status.ToLower() == "blocked")
                    query = query.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow);
            }

            // Filtro por rango de fechas de creación
            if (searchParams.CreatedFrom.HasValue)
                query = query.Where(u => u.RegisteredAt >= searchParams.CreatedFrom.Value);
            
            if (searchParams.CreatedTo.HasValue)
                query = query.Where(u => u.RegisteredAt <= searchParams.CreatedTo.Value);

            // Filtro por rol (requiere join con UserRoles)
            if (!string.IsNullOrWhiteSpace(searchParams.Role))
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name == searchParams.Role)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId != 0)
                {
                    query = query.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
                }
            }

            var totalCount = await query.CountAsync();

            // Ordenamiento con whitelist
            var allowedOrderFields = new[] { "createdAt", "lastLogin", "email" };
            var orderBy = searchParams.OrderBy?.ToLower();
            var orderDir = searchParams.OrderDir?.ToLower() ?? "desc";

            if (!string.IsNullOrWhiteSpace(orderBy) && allowedOrderFields.Contains(orderBy))
            {
                query = orderBy switch
                {
                    "createdat" => orderDir == "asc" 
                        ? query.OrderBy(u => u.RegisteredAt) 
                        : query.OrderByDescending(u => u.RegisteredAt),
                    "lastlogin" => orderDir == "asc" 
                        ? query.OrderBy(u => u.LastLoginTime) 
                        : query.OrderByDescending(u => u.LastLoginTime),
                    "email" => orderDir == "asc" 
                        ? query.OrderBy(u => u.Email) 
                        : query.OrderByDescending(u => u.Email),
                    _ => query.OrderByDescending(u => u.RegisteredAt)
                };
            }
            else
            {
                query = query.OrderByDescending(u => u.RegisteredAt);
            }

            // Paginación
            var pageSize = searchParams.PageSize ?? 10;
            var users = await query
                .Skip((searchParams.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        /// <summary>
        /// Obtiene un usuario por ID con información completa para administradores.
        /// </summary>
        public async Task<User?> GetUserForAdminAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Obtiene el rol de un usuario por ID.
        /// </summary>
        public async Task<string?> GetUserRoleByIdAsync(int id)
        {
            var userRole = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .FirstOrDefaultAsync();

            return userRole;
        }
    }
}