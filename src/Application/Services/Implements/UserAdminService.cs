using Mapster;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
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
                throw new BadRequestAppException("Campo de ordenamiento no válido.");
            }

            var (users, totalCount) = await _userRepository.GetUsersForAdminAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var userDtos = new List<UserForAdminDTO>();
            foreach (var user in users)
            {
                var role = await _userRepository.GetUserRoleByIdAsync(user.Id) ?? "Sin rol";
                var status = GetUserStatus(user);

                var dto = new UserForAdminDTO
                {
                    Id = user.Id,
                    Name = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Role = role,
                    Status = status,
                    CreatedAt = user.RegisteredAt,
                    LastLogin = user.LastLoginTime
                };
                userDtos.Add(dto);
            }

            Log.Information("Listado de usuarios obtenido para admin. Total: {TotalCount}, Página: {PageNumber}",
                totalCount, searchParams.PageNumber);

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

            var role = await _userRepository.GetUserRoleByIdAsync(id) ?? "Sin rol";
            var status = GetUserStatus(user);

            var dto = new UserDetailForAdminDTO
            {
                Id = user.Id,
                Name = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Rut = user.Rut,
                PhoneNumber = user.PhoneNumber,
                Role = role,
                Status = status,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.RegisteredAt,
                UpdatedAt = user.UpdatedAt,
                LastLogin = user.LastLoginTime
            };

            Log.Information("Detalle de usuario {UserId} obtenido para admin", id);
            return dto;
        }

        private static string GetUserStatus(Domain.Models.User user)
        {
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                return "Bloqueado";

            return "Activo";
        }
    }
}