using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 // Ajusta según tu proyecto
using TiendaProyecto.src.Exceptions; // Tus excepciones personalizadas
using Serilog;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
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
    }
}