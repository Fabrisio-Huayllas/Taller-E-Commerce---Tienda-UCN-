using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.Services.Interfaces;
// Ajusta según tu proyecto
using TiendaProyecto.src.Exceptions; // Tus excepciones personalizadas


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
        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var message = await _userService.RegisterAsync(registerDTO, HttpContext);
            return Ok(new GenericResponse<string>("Registro exitoso", message));
        }


        /// <summary>
        /// Verifica el correo electrónico del usuario.
        /// </summary>
        /// <param name="verifyEmailDTO">DTO que contiene el correo electrónico y el código de verificación.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
        {
            var message = await _userService.VerifyEmailAsync(verifyEmailDTO);
            return Ok(new GenericResponse<string>("Verificación de correo electrónico exitosa", message));
        }

        /// <summary>
        /// Reenvía el código de verificación al correo electrónico del usuario.
        /// </summary>
        /// <param name="resendEmailVerificationCodeDTO">DTO que contiene el correo electrónico del usuario.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("resend-email-verification-code")]
        public async Task<IActionResult> ResendEmailVerificationCode([FromBody] ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO)
        {
            var message = await _userService.ResendEmailVerificationCodeAsync(resendEmailVerificationCodeDTO);
            return Ok(new GenericResponse<string>("Código de verificación reenviado exitosamente", message));
        }
    }

}