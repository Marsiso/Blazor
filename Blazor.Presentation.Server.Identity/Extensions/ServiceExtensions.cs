using Blazor.Shared.Entities.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Presentation.Server.Identity.Extensions;

internal static class ServiceExtensions
{
    internal static IServiceCollection ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options => options.AddPolicy("Default", builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        }));

    internal static IServiceCollection ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });

    internal static IServiceCollection ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<SqlContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("Default"), b =>
            b.MigrationsAssembly(typeof(Program).Assembly.FullName)));
}