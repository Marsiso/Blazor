using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Responses;
using Blazor.Shared.Implementations.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Blazor.Presentation.Server.Extensions;

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

    internal static IServiceCollection ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    internal static void ConfigureExceptionHandler(this IApplicationBuilder builder, Serilog.ILogger logger)
    {
        _ = builder.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger.Error("Something went wrong: {Error}", contextFeature.Error);

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error."
                    }.ToString());
                }
            });
        });
    }

    internal static AuthorizationPolicy FromCzechiaPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Czechia")
        .Build();
}
