using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using Serilog;
using System.Text;
using TiendaProyecto.src.Application.Mappers;
using TiendaProyecto.src.Application.Services.Implements;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Infrastructure.Repositories.Implements;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using TiendaProyecto.src.Middleware;



var builder = WebApplication.CreateBuilder(args);
MapperExtensions.ConfigureMapster();

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


#region Email Service Configuration
Log.Information("Configurando servicio de Email");
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["ResendAPIKey"] ?? throw new InvalidOperationException("El token de API de Resend no está configurado.");
});
builder.Services.AddTransient<IResend, ResendClient>();
#endregion



#region Authentication Configuration
Log.Information("Configurando autenticación JWT");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(options =>
    {
        string jwtSecret = builder.Configuration["JWTSecret"] ?? throw new InvalidOperationException("La clave secreta JWT no está configurada.");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero //Sin tolerencia a tokens expirados
        };
    });
#endregion    

// Activar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Registro del servicio de token
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
// Configurar Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tienda API", Version = "v1" });
});

#region Configuración de Identity
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
#endregion

#region Configuración de la base de datos SQLite
Log.Information("Configurando base de datos SQLite");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));
#endregion

var app = builder.Build();

#region Aplicar migraciones
Log.Information("Aplicando migraciones a la base de datos");
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
}
#endregion

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();