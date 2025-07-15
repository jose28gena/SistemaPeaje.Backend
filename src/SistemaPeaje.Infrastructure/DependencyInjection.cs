using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SistemaPeaje.Infrastructure.Data;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Infrastructure.Repositories;
using SistemaPeaje.Infrastructure.Services;

namespace SistemaPeaje.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SistemaPeaje.Infrastructure")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILiquidacionService, LiquidacionService>();

        return services;
    }
}
