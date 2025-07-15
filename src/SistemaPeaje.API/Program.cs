using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Application;
using SistemaPeaje.Infrastructure;
using SistemaPeaje.Infrastructure.Data;
using SistemaPeaje.API.Middleware;
using Serilog;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static SistemaPeaje.Infrastructure.Data.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Sistema Peaje API",
        Version = "v1",
        Description = "API para el sistema de gestión de peajes"
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4203", "https://localhost:4203")
                .WithOrigins("http://localhost:4201", "https://localhost:4201")
                    .WithOrigins("http://localhost:4202", "https://localhost:4202")
                    .WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Registrar servicios de PLC
builder.Services.AddScoped<SistemaPeaje.Core.Interfaces.IPlcModbusService, SistemaPeaje.Infrastructure.Services.PlcModbusService>();
builder.Services.AddScoped<SistemaPeaje.Core.Interfaces.IPlcConfiguracionService, SistemaPeaje.Infrastructure.Services.PlcConfiguracionService>();

// Registrar el Manager Service para múltiples PLCs (reemplaza al worker individual)
builder.Services.AddSingleton<SistemaPeaje.Infrastructure.Workers.PlcManagerService>();
builder.Services.AddHostedService<SistemaPeaje.Infrastructure.Workers.PlcManagerService>(provider =>
    provider.GetRequiredService<SistemaPeaje.Infrastructure.Workers.PlcManagerService>());

// Configurar validaciones con FluentValidation
// builder.Services.AddFluentValidationAutoValidation(); // Commented out - may need different package
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Peaje API V1");
        c.RoutePrefix = string.Empty; // Para que Swagger sea la página principal
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData.SeedAsync(context);
}

try
{
    Log.Information("Starting Sistema Peaje API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Sistema Peaje API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
