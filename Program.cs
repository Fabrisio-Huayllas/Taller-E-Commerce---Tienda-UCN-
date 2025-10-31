using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Storage.SQLite;
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
using TiendaProyecto.src.Application.Jobs.Implements;
using TiendaProyecto.src.Application.Mappers;
using TiendaProyecto.src.Application.Services.Implements;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Infrastructure.Repositories.Implements;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Exceptions;
using TiendaProyecto.src.Infrastructure.Data;
using TiendaProyecto.src.Middleware;




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

        // Configurar eventos para manejar errores de autenticación
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Evitar que el middleware de autenticación maneje el error
                context.HandleResponse();

                // Lanzar una excepción personalizada para que sea manejada por el ErrorHandlerMiddleware
                throw new UnauthorizedAppException("No autorizado.");
            }
        };
    });
#endregion    

//Mappers
builder.Services.AddScoped<ProductMapper>();
builder.Services.AddScoped<UserMapper>();
builder.Services.AddScoped<CartMapper>();
builder.Services.AddScoped<OrderMapper>();

// Activar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Registro del servicio de token
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


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

#region Hangfire Configuration
Log.Information("Configurando los trabajos en segundo plano de Hangfire");
var cronExpression = builder.Configuration["Jobs:CronJobDeleteUnconfirmedUsers"] ?? throw new InvalidOperationException("La expresión cron para eliminar usuarios no confirmados no está configurada.");
var timeZone = TimeZoneInfo.FindSystemTimeZoneById(builder.Configuration["Jobs:TimeZone"] ?? throw new InvalidOperationException("La zona horaria para los trabajos no está configurada."));
builder.Services.AddHangfire(configuration =>
{
    var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);
    var databasePath = connectionStringBuilder.DataSource;

    configuration.UseSQLiteStorage(databasePath);
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
    configuration.UseSimpleAssemblyNameTypeSerializer();
    configuration.UseRecommendedSerializerSettings();
});
builder.Services.AddHangfireServer();


#endregion

// Agregar política de autorización para administradores
builder.Services.AddAuthorization(opts => {
  opts.AddPolicy("Admin", p => p.RequireRole("Admin"));
});

var app = builder.Build();
// Configurar el panel de control de Hangfire
app.UseHangfireDashboard(builder.Configuration["HangfireDashboard:DashboardPath"] ?? throw new InvalidOperationException("La ruta de hangfire no ha sido declarada"), new DashboardOptions
{
    StatsPollingInterval = builder.Configuration.GetValue<int?>("HangfireDashboard:StatsPollingInterval") ?? throw new InvalidOperationException("El intervalo de actualización de estadísticas del panel de control de Hangfire no está configurado."),
    DashboardTitle = builder.Configuration["HangfireDashboard:DashboardTitle"] ?? throw new InvalidOperationException("El título del panel de control de Hangfire no está configurado."),
    DisplayStorageConnectionString = builder.Configuration.GetValue<bool?>("HangfireDashboard:DisplayStorageConnectionString") ?? throw new InvalidOperationException("La configuración 'HangfireDashboard:DisplayStorageConnectionString' no está definida."),
});


#region Database Migration and jobs Configuration
Log.Information("Aplicando migraciones a la base de datos");
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
    var jobId = nameof(UserJob.DeleteUnconfirmedAsync);
    RecurringJob.AddOrUpdate<UserJob>(
        jobId,
        job => job.DeleteUnconfirmedAsync(),
        cronExpression,
        new RecurringJobOptions
        {
            TimeZone = timeZone
        }
    );
    Log.Information($"Job recurrente '{jobId}' configurado con cron: {cronExpression} en zona horaria: {timeZone.Id}");
    MapperExtensions.ConfigureMapster(scope.ServiceProvider);
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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CartMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();