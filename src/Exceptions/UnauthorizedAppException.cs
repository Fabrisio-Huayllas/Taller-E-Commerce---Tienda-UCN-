using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
    /// <summary>
    /// Excepción personalizada para representar errores de acceso no autorizado.
    /// </summary>
    /// <remarks>
    /// Contiene un código de error opcional que por defecto es "NOT_FOUND".
    /// </remarks>
    public class UnauthorizedAppException : Exception
    {
        /// <summary>
        /// Código de error asociado a la excepción de acceso no autorizado.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Constructor de la excepción Unauthorized.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del error de acceso.</param>
        /// <param name="errorCode">Código de error opcional (por defecto "NOT_FOUND").</param>
        public UnauthorizedAppException(string message, string? errorCode = "NOT_FOUND") 
            : base(message)
        {
            ErrorCode = errorCode ?? "NOT_FOUND";
        }
    }
}
