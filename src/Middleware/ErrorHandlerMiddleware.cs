using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using TiendaProyecto.src.Exceptions;

namespace TiendaProyecto.src.Middleware
{
    /// <summary>
    /// Middleware para manejo centralizado de errores en la aplicación.
    /// </summary>
    /// <remarks>
    /// Captura excepciones no manejadas, mapea a códigos HTTP y devuelve un payload JSON estandarizado
    /// con información de error, traceId y detalles de la solicitud.
    /// </remarks>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
         /// <summary>
        /// Constructor del middleware.
        /// </summary>
        /// <param name="next">Delegate para invocar el siguiente middleware en la pipeline.</param>
        /// <param name="logger">Logger para registrar excepciones.</param>
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// Método que intercepta la ejecución del pipeline y captura excepciones.
        /// </summary>
        /// <param name="context">Contexto HTTP de la solicitud actual.</param>
        /// <returns>Tarea asincrónica representando la ejecución del middleware.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
        {
            var (status, code, message, errors) = MapException(ex);

            _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId} Path: {Path}", ctx.TraceIdentifier, ctx.Request.Path);

            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)status;

            var payload = new
            {
                status = ctx.Response.StatusCode,
                code,
                message,
                traceId = ctx.TraceIdentifier,
                path = ctx.Request.Path.Value,
                method = ctx.Request.Method,
                timestamp = DateTime.UtcNow,
                errors
            };

            await ctx.Response.WriteAsJsonAsync(payload);
        }
        private static (HttpStatusCode, string, string, object?) MapException(Exception ex)
        {
            return ex switch
            {
                NotFoundException nf => (HttpStatusCode.NotFound, nf.ErrorCode, nf.Message, null),
                ConflictException cf => (HttpStatusCode.Conflict, cf.ErrorCode, cf.Message, null),
                ForbiddenException fb => (HttpStatusCode.Forbidden, fb.ErrorCode, fb.Message, null),
                UnauthorizedAppException ua => (HttpStatusCode.Unauthorized, ua.ErrorCode, ua.Message, null),
                BadRequestAppException br => (HttpStatusCode.BadRequest, br.ErrorCode, br.Message, br.Errors),
                AppException ae => (HttpStatusCode.BadRequest, "APP_ERROR", ae.Message, null),
                ValidationException fv => (
                    HttpStatusCode.BadRequest,
                    "VALIDATION_ERROR",
                    "One or more validation errors occurred.",
                    fv.Errors.GroupBy(e => e.PropertyName)
                             .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                ),
                KeyNotFoundException knf => (HttpStatusCode.NotFound, "NOT_FOUND", knf.Message, null),
                InvalidOperationException ioe => (HttpStatusCode.BadRequest, "INVALID_OPERATION", ioe.Message, null),
                ArgumentException ae => (HttpStatusCode.BadRequest, "INVALID_ARGUMENT", ae.Message, null),
                TimeoutException te => (HttpStatusCode.TooManyRequests, "TIMEOUT", te.Message, null),

                _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An internal server error occurred.", null),


            };
        }
    }
}