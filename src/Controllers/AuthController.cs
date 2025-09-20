using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Dtos; // Ajusta según tu proyecto
using TiendaProyecto.src.Exceptions; // Tus excepciones personalizadas
namespace TiendaProyecto.src.Controllers
{
    [ApiController]
    [Route("api/auth")]
     public class AuthController : ControllerBase
    {
        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto dto)
        {
            // La validación con FluentValidation ocurre automáticamente
            // si tu DTO es inválido, ASP.NET devuelve 400 con InvalidModelStateResponseFactory
            // Simulamos un registro exitoso:
            return Created("", new { success = true, message = "Cuenta creada. Verifique su correo." });
        }

        // GET: api/auth/test-notfound
        [HttpGet("test-notfound")]
        public IActionResult TestNotFound()
        {
            throw new NotFoundException("Usuario no encontrado");
        }

        // GET: api/auth/test-error
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Error interno simulado");
        }
    }
}