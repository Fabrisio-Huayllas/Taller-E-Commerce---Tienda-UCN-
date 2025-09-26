using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
   /// <summary>
    /// Excepción personalizada para errores de la aplicación.
    /// </summary>
    public class AppException : Exception
    {
        public AppException() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }
}