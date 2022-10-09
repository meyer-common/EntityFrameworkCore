using Meyer.Common.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boosted.Entities;

public static class DbContextModule
{
    public static IServiceCollection AddDbConnectionOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddScoped(x => configuration.GetSection(DbConnectionOptions.SectionName).Get<DbConnectionOptions>());
    }

    public static IServiceCollection AddMultiTenantDbContextFactory<T>(this IServiceCollection services) where T : DbContextBase
    {
        return services
            .AddScoped<IMultiTenantDbContextFactory<T>, MultiTenantDbContextFactory<T>>()
            .AddDbContextFactory<T>(x => { }, ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddSingleTenantDbContextPool<T>(this IServiceCollection services) where T : DbContextBase
    {
        return services.AddDbContextPool<T>(x => { });
    }
}