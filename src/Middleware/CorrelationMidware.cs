using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Middleware;


namespace TiendaProyecto.src.Middleware
{
    /// <summary>
    /// Middleware para gestionar el Correlation ID en las solicitudes HTTP.
    /// </summary>
    /// <remarks>
    /// Genera un identificador único por cada petición si no se provee, lo agrega a los headers de la respuesta
    /// y lo asigna a TraceIdentifier para seguimiento y trazabilidad de logs.
    /// Compatible con .NET 9.
    /// </remarks>
    public class CorrelationMidware
    {
        private const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

         /// <summary>
        /// Constructor del middleware.
        /// </summary>
        /// <param name="next">Delegate para invocar el siguiente middleware en la pipeline.</param>
        public CorrelationMidware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

       /// <summary>
        /// Método que maneja la ejecución del middleware.
        /// </summary>
        /// <param name="context">Contexto HTTP de la solicitud actual.</param>
        /// <returns>Tarea asincrónica representando la ejecución del middleware.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var cid) || string.IsNullOrWhiteSpace(cid))
            {
                cid = Guid.NewGuid().ToString("N");
            }

            context.Response.Headers[HeaderName] = cid;
            context.TraceIdentifier = cid!;

            await _next(context);
        }
    }
}