using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.DTO.BaseResponse;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Exceptions;

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
        /// Cambia la contraseña del usuario autenticado.
        /// </summary>
        /// <param name="dto">DTO con la contraseña actual y nueva.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Contraseña actualizada correctamente." });
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

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// </summary>
        /// <returns>Un IActionResult con los datos del perfil del usuario.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var profile = await _userService.GetProfileAsync(userId);
            return Ok(profile);
        }

        /// <summary>
        /// Actualiza el perfil del usuario autenticado.
        /// </summary>
        /// <param name="dto">DTO con los datos actualizados del perfil.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _userService.UpdateProfileAsync(userId, dto);
            return Ok(new { message = "Perfil actualizado correctamente." });
        }

        /// <summary>
        /// Solicita el envío del código de recuperación de contraseña al correo electrónico del usuario.
        /// </summary>
        /// <param name="dto">DTO que contiene el correo electrónico del usuario.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] ResendEmailVerificationCodeDTO dto)
        {
            var message = await _userService.SendPasswordRecoveryCodeAsync(dto.Email);
            return Ok(new GenericResponse<string>("Código de recuperación enviado exitosamente", message));
        }

        /// <summary>
        /// Restablece la contraseña del usuario.
        /// </summary>
        /// <param name="dto">DTO que contiene la nueva contraseña y el token de recuperación.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await _userService.ResetPasswordAsync(dto);
            return Ok(new GenericResponse<string>("Contraseña restablecida exitosamente"));
        }
    }
}

