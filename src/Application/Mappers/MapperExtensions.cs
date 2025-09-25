using Mapster;
using TiendaProyecto.src.Application.Mappers;

namespace TiendaProyecto.src.Application.Mappers
{
    /// <summary>
    /// Clase para extensiones de mapeo.
    /// Contiene configuraciones globales de mapeo.
    /// </summary>
    public class MapperExtensions
    {
        /// <summary>
        /// Configura los mapeos globales.
        /// </summary>
        public static void ConfigureMapster()
        {
            UserMapper.ConfigureAllMappings();

            // Configuración global de Mapster para ignorar valores nulos
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
        }
    }
}