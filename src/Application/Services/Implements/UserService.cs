using Microsoft.AspNetCore.Http;
using Serilog;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio de usuarios mínimo para login con JWT.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserService(ITokenService tokenService, IUserRepository userRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO con email, contraseña y rememberMe.</param>
        /// <param name="httpContext">Contexto HTTP actual.</param>
        /// <returns>Token JWT y userId.</returns>
        public async Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";

            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                Log.Warning($"Login fallido. Usuario no encontrado: {loginDTO.Email} desde IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var passwordValid = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
            if (!passwordValid)
            {
                Log.Warning($"Login fallido. Contraseña incorrecta para {loginDTO.Email} desde IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            string roleName = await _userRepository.GetUserRoleAsync(user)
                ?? throw new InvalidOperationException("El usuario no tiene rol asignado.");

            var token = _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);

            Log.Information($"Login exitoso para {loginDTO.Email} desde IP: {ipAddress}");
            return (token, user.Id);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            var passwordValid = await _userRepository.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!passwordValid)
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

            var result = await _userRepository.UpdatePasswordAsync(user, dto.NewPassword);
            if (!result)
                throw new Exception("No se pudo actualizar la contraseña.");
        }

        /// <summary>
        /// (Opcional) Elimina usuarios no confirmados.
        /// </summary>
        public async Task<int> DeleteUnconfirmedAsync()
        {
            return await _userRepository.DeleteUnconfirmedAsync();
        }
    }
}