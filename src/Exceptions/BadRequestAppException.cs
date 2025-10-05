using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
    /// <summary>
    /// Excepción personalizada para representar errores de tipo "Bad Request" en la aplicación.
    /// </summary>
    /// <remarks>
    /// Contiene un código de error y un diccionario opcional de errores detallados por campo.
    /// </remarks>
    public class BadRequestAppException : Exception
    {
        /// <summary>
        /// Código de error asociado a la excepción.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Diccionario opcional que contiene errores detallados por campo.
        /// </summary>
        public IDictionary<string, string[]>? Errors { get; }
         /// <summary>
        /// Constructor de la excepción.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del error.</param>
        /// <param name="errors">Diccionario opcional de errores por campo.</param>
        /// <param name="errorCode">Código de error opcional (por defecto "BAD_REQUEST").</param>
        public BadRequestAppException(
            string message,
            IDictionary<string, string[]>? errors = null,
            string? errorCode = "BAD_REQUEST"
        ) : base(message)
        {
            ErrorCode = errorCode ?? "BAD_REQUEST";
            Errors = errors;
        }
    }
}