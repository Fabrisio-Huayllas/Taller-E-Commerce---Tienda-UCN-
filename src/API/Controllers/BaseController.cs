using Microsoft.AspNetCore.Mvc;

namespace TiendaProyecto.src.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    /// <summary>
    /// Controlador base que proporciona la configuración común para todos los controladores de la API.
    /// </summary>
    /// <remarks>
    /// Aplica el atributo <see cref="ApiControllerAttribute"/> para habilitar las características automáticas de validación de modelos 
    /// y el enrutamiento con prefijo "api/[controller]".
    /// </remarks>
    public class BaseController : ControllerBase;
}