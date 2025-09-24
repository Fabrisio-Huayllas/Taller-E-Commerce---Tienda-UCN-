using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Exceptions; 
using Serilog;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace TiendaProyecto.src.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <returns>Token JWT y el ID del usuario si el login es exitoso.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var (token, userId) = await _userService.LoginAsync(loginDTO, HttpContext);

            Log.Information("Usuario {Email} inició sesión correctamente.", loginDTO.Email);

            return Ok(new
            {
                success = true,
                token,
                userId
            });
        }
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Contraseña actualizada correctamente." });
        }
    }
}