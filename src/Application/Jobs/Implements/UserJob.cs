using Hangfire;
using Serilog;
using TiendaProyecto.src.Application.Jobs.Interfaces;
using TiendaProyecto.src.Application.Services.Interfaces;

namespace TiendaProyecto.src.Application.Jobs.Implements
{
    /// <summary>
    /// Clase que gestiona trabajos relacionados con usuarios utilizando Hangfire.
    /// </summary>
    /// <remarks>
    /// Implementa tareas periódicas, como la eliminación de usuarios no confirmados, con reintentos automáticos en caso de fallo.
    /// </remarks>
    public class UserJob : IUserJob
    {
        
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor que inyecta los servicios necesarios.
        /// </summary>
        /// <param name="userService">Servicio de gestión de usuarios.</param>
        /// <param name="_configuration">Configuración de la aplicación (actualmente no utilizada).</param>

        public UserJob(IUserService userService, IConfiguration _configuration)
        {
            _userService = userService;

        }

        [AutomaticRetry(Attempts = 10, DelaysInSeconds = new int[] { 60, 120, 300, 600, 900 })]
        /// <summary>
        /// Elimina usuarios que no han confirmado su cuenta.
        /// </summary>
        /// <remarks>
        /// Aplica reintentos automáticos en caso de fallo, con intervalos crecientes.
        /// </remarks>
        public async Task DeleteUnconfirmedAsync()
        {

            Log.Information("Eliminando usuarios no confirmados...");
            await _userService.DeleteUnconfirmedAsync();
        }
    }
}