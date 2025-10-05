using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
    /// <summary>
    /// Excepción personalizada para representar errores de recurso no encontrado (Not Found).
    /// </summary>
    /// <remarks>
    /// Contiene un código de error opcional que por defecto es "NOT_FOUND".
    /// </remarks>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Código de error asociado a la excepción de recurso no encontrado.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Constructor de la excepción NotFound.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del error.</param>
        /// <param name="errorCode">Código de error opcional (por defecto "NOT_FOUND").</param>
        public NotFoundException(string message, string? errorCode = "NOT_FOUND") 
            : base(message)
        {
            ErrorCode = errorCode ?? "NOT_FOUND";
        }
    }
}
