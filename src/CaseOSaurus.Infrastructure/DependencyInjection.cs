using CaseOSaurus.Application.Common.Interfaces;
using CaseOSaurus.Application.Common.Services;
using CaseOSaurus.Infrastructure.Persistence;
using CaseOSaurus.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaseOSaurus.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services. If skipDb is true, the DbContext and IApplicationDbContext registration are skipped
    /// so tests can replace the database provider without conflicting with EF Core service registrations.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool skipDb)
    {
        if (!skipDb)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }

        services.AddScoped<IUserContext, UserContext>();

        services.AddHttpContextAccessor();

        return services;
    }
}
