using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Exceptions
{
    public class BadRequestAppException: Exception
    {
        public string ErrorCode { get; }

        // Diccionario de errores opcionales 
        public IDictionary<string, string[]>? Errors { get; }

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