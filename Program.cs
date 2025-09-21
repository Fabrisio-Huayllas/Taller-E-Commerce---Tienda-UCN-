using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Middleware;
using TiendaProyecto.src.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

// Configuración de la conexión a la base de datos SQLite
var connectionString = builder.Configuration.GetConnectionString("SqliteDatabase") 
    ?? throw new InvalidOperationException("Connection string SqliteDatabase no configurado");

// Agregar servicios de controllers y validación automática
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var http = context.HttpContext;
            var errors = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var problem = new
            {
                status = StatusCodes.Status400BadRequest,
                code = "VALIDATION_ERROR",
                message = "One or more validation errors occurred.",
                traceId = http.TraceIdentifier,
                path = http.Request.Path.Value,
                method = http.Request.Method,
                timestamp = DateTime.UtcNow,
                errors
            };

            return new BadRequestObjectResult(problem);
        };
    });

// Activar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configurar Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tienda API", Version = "v1" });
});

// Configuración de Identity
Log.Information("Configurando Identity");
builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;

    options.User.AllowedUserNameCharacters = builder.Configuration["IdentityConfiguration:AllowedUserNameCharacters"] 
        ?? throw new InvalidOperationException("Los caracteres permitidos para UserName no están configurados.");
})
.AddRoles<Role>()
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Configuración de la base de datos SQLite
Log.Information("Configurando base de datos SQLite");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Aplicar migraciones
Log.Information("Aplicando migraciones a la base de datos");
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
}

// Middlewares globales
app.UseMiddleware<CorrelationMidware>();
app.UseMiddleware<ErrorHandlerMiddleware>();

// Activar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapear endpoints de controllers
app.MapControllers();

// Endpoint minimal API de ejemplo
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

// Modelo para minimal API
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
