using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Xml.Schema;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;


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

            // Capturar el tiempo actual antes de usarlo en la consulta
            var currentTime = DateTimeOffset.UtcNow;

            // Filtro por email (búsqueda parcial)
            if (!string.IsNullOrWhiteSpace(searchParams.Email))
            {
                var emailSearch = searchParams.Email.Trim().ToLower();
                query = query.Where(u => u.Email!.ToLower().Contains(emailSearch));
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

            // Materializar la consulta antes de aplicar filtro de estado (problema con DateTimeOffset en SQLite)
            var allUsers = await query.ToListAsync();

            // Aplicar filtro por estado en memoria
            IEnumerable<User> filteredUsers = allUsers;
            if (!string.IsNullOrWhiteSpace(searchParams.Status))
            {
                var statusLower = searchParams.Status.ToLower();
                if (statusLower == "active")
                    filteredUsers = allUsers.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= currentTime);
                else if (statusLower == "blocked")
                    filteredUsers = allUsers.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > currentTime);
            }

            var totalCount = filteredUsers.Count();

            // Ordenamiento con whitelist (todas las opciones en minúsculas para comparación)
            var allowedOrderFields = new[] { "createdat", "lastlogin", "email" };
            var orderBy = searchParams.OrderBy?.ToLower();
            var orderDir = searchParams.OrderDir?.ToLower() ?? "desc";

            if (!string.IsNullOrWhiteSpace(orderBy) && allowedOrderFields.Contains(orderBy))
            {
                filteredUsers = orderBy switch
                {
                    "createdat" => orderDir == "asc"
                        ? filteredUsers.OrderBy(u => u.RegisteredAt)
                        : filteredUsers.OrderByDescending(u => u.RegisteredAt),
                    "lastlogin" => orderDir == "asc"
                        ? filteredUsers.OrderBy(u => u.LastLoginTime)
                        : filteredUsers.OrderByDescending(u => u.LastLoginTime),
                    "email" => orderDir == "asc"
                        ? filteredUsers.OrderBy(u => u.Email)
                        : filteredUsers.OrderByDescending(u => u.Email),
                    _ => filteredUsers.OrderByDescending(u => u.RegisteredAt)
                };
            }
            else
            {
                filteredUsers = filteredUsers.OrderByDescending(u => u.RegisteredAt);
            }

            // Paginación
            var pageNumber = searchParams.PageNumber ?? 1;
            var pageSize = searchParams.PageSize ?? 10;
            var users = filteredUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

        /// <summary>
        /// Actualiza el estado de un usuario (activo/bloqueado).
        /// </summary>
        public async Task<bool> UpdateUserStatusAsync(int userId, bool isBlocked)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            if (isBlocked)
            {
                // Bloquear por 100 años (prácticamente permanente)
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            }
            else
            {
                // Desbloquear
                user.LockoutEnd = null;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cuenta cuántos administradores activos hay en el sistema.
        /// </summary>
        public async Task<int> CountActiveAdminsAsync()
        {
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null) return 0;

            return await _context.UserRoles
                .Where(ur => ur.RoleId == adminRole.Id)
                .Join(_context.Users,
                    ur => ur.UserId,
                    u => u.Id,
                    (ur, u) => u)
                .Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow)
                .CountAsync();
        }

        /// <summary>
        /// Registra un cambio de estado en la auditoría.
        /// </summary>
        public async Task<bool> CreateStatusAuditAsync(UserStatusAudit audit)
        {
            _context.UserStatusAudits.Add(audit);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Invalida las sesiones activas de un usuario.
        /// </summary>
        public async Task<bool> InvalidateUserSessionsAsync(int userId)
        {
            // En una implementación real, aquí invalidarías tokens JWT activos
            // Por ahora, actualizamos un timestamp que podría usarse para validar tokens
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.UpdatedAt = DateTime.UtcNow; // Los tokens anteriores a esta fecha se consideran inválidos
            await _context.SaveChangesAsync();

            Log.Information("Sesiones invalidadas para usuario {UserId}", userId);
            return true;
        }
    }
}