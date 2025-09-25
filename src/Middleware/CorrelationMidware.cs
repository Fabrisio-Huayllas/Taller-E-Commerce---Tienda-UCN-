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
    public class CorrelationMidware
    {
        private const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationMidware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        // Cambiado a Invoke para compatibilidad .NET 9
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var cid) || string.IsNullOrWhiteSpace(cid))
            {
                cid = Guid.NewGuid().ToString("N");
            }

            context.Response.Headers[HeaderName] = cid;
            context.TraceIdentifier = cid;

            await _next(context);
        }
    }
}