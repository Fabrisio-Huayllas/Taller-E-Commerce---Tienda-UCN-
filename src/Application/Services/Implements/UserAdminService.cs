using Mapster;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;

namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para la administración de usuarios.
    /// </summary>
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public UserAdminService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<ListedUsersForAdminDTO> GetUsersAsync(UserSearchParamsDTO searchParams)
        {
            // Validar parámetros de entrada
            if (searchParams.PageNumber <= 0)
                searchParams.PageNumber = 1;

            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (pageSize > 100) pageSize = 100; // Límite máximo

            // Validar ordenamiento
            var allowedOrderFields = new[] { "createdAt", "lastLogin", "email" };
            if (!string.IsNullOrWhiteSpace(searchParams.OrderBy) &&
                !allowedOrderFields.Contains(searchParams.OrderBy.ToLower()))
            {
                throw new BadRequestAppException("Campo de ordenamiento no válido. Campos permitidos: createdAt, lastLogin, email");
            }

            var (users, totalCount) = await _userRepository.GetUsersForAdminAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Mapear a DTOs
            var userDtos = new List<UserForAdminDTO>();
            foreach (var user in users)
            {
                var userRole = await _userRepository.GetUserRoleByIdAsync(user.Id);
                var userDto = user.Adapt<UserForAdminDTO>();
                userDto.Role = userRole ?? "Sin rol";
                userDto.Status = GetUserStatus(user);
                userDtos.Add(userDto);
            }

            return new ListedUsersForAdminDTO
            {
                Users = userDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = searchParams.PageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UserDetailForAdminDTO> GetUserDetailAsync(int id)
        {
            var user = await _userRepository.GetUserForAdminAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuario con ID {id} no encontrado.");
            }

            var userRole = await _userRepository.GetUserRoleByIdAsync(id);
            var userDto = user.Adapt<UserDetailForAdminDTO>();
            userDto.Role = userRole ?? "Sin rol";
            userDto.Status = GetUserStatus(user);

            return userDto;
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDTO updateDto, int adminId)
        {
            // Verificar que el usuario existe
            var user = await _userRepository.GetUserForAdminAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"Usuario con ID {userId} no encontrado.");
            }

            // Regla de negocio: No permitir auto-bloqueo
            if (userId == adminId && updateDto.Status.ToLower() == "blocked")
            {
                throw new ConflictException("No puedes bloquearte a ti mismo.");
            }

            // Obtener estado actual
            var currentStatus = GetUserStatus(user);
            var newStatus = updateDto.Status.ToLower() == "blocked" ? "Bloqueado" : "Activo";

            // Verificar si ya está en el estado solicitado (idempotencia)
            if (currentStatus == newStatus)
            {
                return true; // Ya está en el estado solicitado
            }

            // Regla de negocio: No dejar el sistema sin administradores
            if (updateDto.Status.ToLower() == "blocked")
            {
                var userRole = await _userRepository.GetUserRoleByIdAsync(userId);
                if (userRole == "Admin")
                {
                    var activeAdminsCount = await _userRepository.CountActiveAdminsAsync();
                    if (activeAdminsCount <= 1)
                    {
                        throw new ConflictException("No se puede bloquear al último administrador del sistema.");
                    }
                }
            }

            // Actualizar estado
            var isBlocked = updateDto.Status.ToLower() == "blocked";
            var updated = await _userRepository.UpdateUserStatusAsync(userId, isBlocked);

            if (!updated)
            {
                return false;
            }

            // Invalidar sesiones si se está bloqueando
            if (isBlocked)
            {
                await _userRepository.InvalidateUserSessionsAsync(userId);
            }

            // Registrar auditoría
            var audit = new UserStatusAudit
            {
                UserId = userId,
                ChangedByAdminId = adminId,
                PreviousStatus = currentStatus,
                NewStatus = newStatus,
                Reason = updateDto.Reason,
                ChangedAt = DateTime.UtcNow
            };

            await _userRepository.CreateStatusAuditAsync(audit);

            Log.Information("Estado de usuario actualizado. UserId: {UserId}, Estado: {NewStatus}, AdminId: {AdminId}",
                userId, newStatus, adminId);

            return true;
        }

        private static string GetUserStatus(Domain.Models.User user)
        {
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                return "Bloqueado";
            return "Activo";
        }
    }
}