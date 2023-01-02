using System.Reflection;
using Blazor.Shared.Entities.Identity;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;
using Blazor.Presentation.Server.Identity.Validators;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Presentation.Server.Identity.Identity;

internal static class IdentityConfiguration
{
    private static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Address(),
        new IdentityResource(name: "roles", displayName: "User role(s)", userClaims: new List<string> { "role" }),
        new IdentityResource(name: "country", displayName: "User country", userClaims: new List<string> { "country" })
    };

    private static IEnumerable<Client> GetClients() => new List<Client>
    {
        new()
        {
            ClientName = "BlazorWasm",
            ClientId = "BlazorWasm",
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,
            RedirectUris = new List<string>{
                "https://localhost:10001/authentication/login-callback"
            },
            PostLogoutRedirectUris = new List<string> {
                "https://localhost:10001/authentication/logout-callback"
        },
            AllowedCorsOrigins = {
            "https://localhost:10001"
        },
            AllowedScopes = {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "BlazorAPI",
                "roles",
                "country"
            }
        }
    };

    static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
    {
        new ApiScope("BlazorAPI", "Blazor API")
    };

    static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
    {
        new ApiResource("BlazorAPI", "Blazor API")
        {
            Scopes = { "BlazorAPI" }, UserClaims = new [] { "country" }
        }
    };

    internal static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        
        services
            .AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration.GetConnectionString("Default"), b =>
                    b.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration.GetConnectionString("Default"), b =>
                    b.MigrationsAssembly(migrationsAssembly));
            })
            .AddResourceOwnerValidator<ApplicationResourceOwnerPasswordValidator>()
            .AddDeveloperSigningCredential();
        
        services.AddControllersWithViews();

        return services;
    }

    internal static IApplicationBuilder InitializeIdentityDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        // Configuration database context
        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();
        if (!context.Clients.Any())
        {
            foreach (var client in GetClients())
            {
                context.Clients.Add(client.ToEntity());
            }
            
            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in GetIdentityResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            
            context.SaveChanges();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var resource in GetApiResources())
            {
                context.ApiResources.Add(resource.ToEntity());
            }
            
            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in GetApiScopes())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }

            context.SaveChanges();
        }

        return app;
    }
}
