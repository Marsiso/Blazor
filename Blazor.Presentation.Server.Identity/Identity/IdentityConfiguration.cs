using System.Reflection;
using Blazor.Shared.Entities.Identity;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Presentation.Server.Identity.Identity;

internal static class IdentityConfiguration
{
    private static List<TestUser> GetUsers() => new()
    {
        new TestUser
        {
            SubjectId = "{223C9865-03BE-4951-8911-740A438FCF9D}",
            Username = "m_olsak@utb.cz",
            Password = "Password9910014785",
            Claims = new List<Claim>
            {
                new("given_name", "Marek"),
                new(JwtClaimTypes.Name, "Marek Olsak"),
                new("family_name", "Olsak"),
                new("address", "Slopne"),
                new("role", Roles.Visitor),
                new("role", Roles.Manager),
                new("role", Roles.Administrator),
                new("country", "Czechia")
            }
        },
        new TestUser
        {
            SubjectId = "{34119795-78A6-44C2-B128-30BFBC29139D}",
            Username = "t_adamek@utb.cz",
            Password = "Password9910014785",
            Claims = new List<Claim>
            {
                new("given_name", "Tomas"),
                new(JwtClaimTypes.Name, "Tomas Adamek"),
                new("family_name", "Adamek"),
                new("address", "Zlin"),
                new("role", Roles.Visitor),
                new("role", Roles.Manager),
                new("country", "France")
            }
        }
    };

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
            // RequireConsent = true
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
        /*services
            .AddIdentityServer()
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddTestUsers(Config.GetUsers())
            .AddInMemoryClients(Config.GetClients())
            .AddDeveloperSigningCredential();*/
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        
        services
            .AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddTestUsers(GetUsers())
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration.GetConnectionString("Default"), b =>
                    b.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(configuration.GetConnectionString("Default"), b =>
                    b.MigrationsAssembly(migrationsAssembly));
            });
        
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
