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
using TiendaProyecto.src.Application.DTO.BaseResponse;


using Microsoft.AspNetCore.Identity;

namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para la administración de usuarios.
    /// </summary>
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly int _defaultPageSize;

        public UserAdminService(IUserRepository userRepository, IConfiguration configuration,UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _defaultPageSize = int.Parse(_configuration["Products:DefaultPageSize"] ?? "10");
        }

        public async Task<ListedUsersForAdminDTO> GetUsersAsync(UserSearchParamsDTO searchParams)
        {
            // Validar parámetros de entrada - establecer valores por defecto
            var pageNumber = searchParams.PageNumber ?? 1;
            if (pageNumber <= 0)
                pageNumber = 1;

            var pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (pageSize > 100) pageSize = 100; // Límite máximo

            // Asignar valores validados al objeto searchParams
            searchParams.PageNumber = pageNumber;
            searchParams.PageSize = pageSize;

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
                CurrentPage = pageNumber,
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
        public async Task<GenericResponse<object>> UpdateUserRoleAsync(int userId, UpdateUserRoleDTO dto, string adminId)
        {
            // R141: Validar que el rol existe en el sistema
            var roleExists = await _userManager.GetUsersInRoleAsync(dto.Role);
            if (roleExists == null)
            {
                throw new BadRequestAppException($"El rol '{dto.Role}' no existe en el sistema");
            }

            // Obtener el usuario a modificar
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Usuario no encontrado");
            }

            // R140: Verificar que el admin tiene permisos
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
            {
                throw new UnauthorizedAppException("No se pudo identificar al administrador");
            }

            var adminRoles = await _userManager.GetRolesAsync(admin);
            if (!adminRoles.Contains("Admin"))
            {
                throw new ForbiddenException("No tiene permisos para realizar esta operación");
            }

            // Obtener el rol actual del usuario
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault() ?? "Sin rol";

            // R141: Validar asignación redundante
            if (currentRole == dto.Role)
            {
                throw new ConflictException($"El usuario ya tiene el rol '{dto.Role}'");
            }

            // R142: Impedir que un Admin se quite a sí mismo el rol si es el último Admin
            if (userId.ToString() == adminId && currentRole == "Admin" && dto.Role != "Admin")
            {
                var adminsInRole = await _userManager.GetUsersInRoleAsync("Admin");
                if (adminsInRole.Count <= 1)
                {
                    throw new ConflictException("No puede quitarse el rol de Administrador siendo el único en el sistema");
                }
            }

            // R143: Registrar auditoría antes del cambio
            user.PreviousRole = currentRole;
            user.LastRoleChangedBy = admin.Email;
            user.LastRoleChangedAt = DateTime.UtcNow;

            // Remover rol actual
            if (!string.IsNullOrEmpty(currentRole) && currentRole != "Sin rol")
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                if (!removeResult.Succeeded)
                {
                    Log.Error("Error removiendo rol {CurrentRole} del usuario {UserId}: {Errors}", 
                        currentRole, userId, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                    throw new BadRequestAppException("No se pudo remover el rol actual del usuario");
                }
            }

            // Asignar nuevo rol
            var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addRoleResult.Succeeded)
            {
                Log.Error("Error asignando rol {NewRole} al usuario {UserId}: {Errors}", 
                    dto.Role, userId, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                throw new BadRequestAppException("No se pudo asignar el nuevo rol al usuario");
            }

            // Actualizar campos de auditoría en la base de datos
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                Log.Error("Error actualizando información de auditoría del usuario {UserId}", userId);
            }

            // R143: Si se reduce privilegios, invalidar sesiones activas
            if (currentRole == "Admin" && dto.Role == "Customer")
            {
                await _userManager.UpdateSecurityStampAsync(user);
                Log.Information("Sesiones invalidadas para usuario {UserId} tras degradación de privilegios", userId);
            }

            Log.Information("Rol actualizado exitosamente para usuario {UserId} de {OldRole} a {NewRole} por {AdminEmail}",
                userId, currentRole, dto.Role, admin.Email);

            return new GenericResponse<object>(
                $"Rol actualizado exitosamente de '{currentRole}' a '{dto.Role}'",
                new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    PreviousRole = currentRole,
                    NewRole = dto.Role,
                    ChangedBy = admin.Email,
                    ChangedAt = user.LastRoleChangedAt
                }
            );
        }
    }
}