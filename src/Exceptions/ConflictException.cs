using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
    /// <summary>
    /// Excepción personalizada para representar conflictos en la aplicación (por ejemplo, recursos duplicados).
    /// </summary>
    /// <remarks>
    /// Contiene un código de error opcional que por defecto es "NOT_FOUND".
    /// </remarks>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Código de error asociado al conflicto.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Constructor de la excepción de conflicto.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del conflicto.</param>
        /// <param name="errorCode">Código de error opcional (por defecto "NOT_FOUND").</param>
        public ConflictException(string message, string? errorCode = "NOT_FOUND") 
            : base(message)
        {
            ErrorCode = errorCode ?? "NOT_FOUND";
        }
    }
}