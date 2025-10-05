using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TiendaProyecto.src.Infrastructure.Data
{
    /// <summary>
    /// Factory para crear instancias de DataContext en tiempo de diseño.
    /// </summary>
    /// <remarks>
    /// Utilizado por herramientas de EF Core como migraciones para instanciar el DbContext sin depender de la inyección de dependencias.
    /// </remarks>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        /// <summary>
        /// Crea una nueva instancia de DataContext con la configuración necesaria.
        /// </summary>
        /// <param name="args">Argumentos opcionales de la línea de comandos.</param>
        /// <returns>Instancia de DataContext configurada.</returns>
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite("Data Source=app.db"); // Usa la misma cadena de conexión que en appsettings.json

            return new DataContext(optionsBuilder.Options);
        }
    }
}