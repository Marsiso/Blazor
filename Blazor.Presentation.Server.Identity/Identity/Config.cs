﻿using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace Blazor.Presentation.Server.Identity.Identity;

internal static class Config
{
    static List<TestUser> GetUsers() => new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "{223C9865-03BE-4951-8911-740A438FCF9D}",
            Username = "m_olsak@utb.cz",
            Password = "Password9910014785",
            Claims = new List<Claim>
            {
                new Claim("given_name", "Marek"),
                new Claim(JwtClaimTypes.Name, "Marek Olsak"),
                new Claim("family_name", "Olsak"),
                new Claim("address", "Slopne"),
                new Claim("role", "admin"),
                new Claim("country", "Czechia")
            }
            },
            new TestUser
            {
                SubjectId = "{34119795-78A6-44C2-B128-30BFBC29139D}",
                Username = "t_adamek@utb.cz",
                Password = "Password9910014785",
                Claims = new List<Claim>
            {
                new Claim("given_name", "Tomas"),
                new Claim(JwtClaimTypes.Name, "Tomas Adamek"),
                new Claim("family_name", "Adamek"),
                new Claim("address", "Zlin"),
                new Claim("role", "admin"),
                new Claim("country", "France")
            }
        }
    };

    static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Address(),
        new IdentityResource(name: "roles", displayName: "User role(s)", userClaims: new List<string> { "role" }),
        new IdentityResource(name: "country", displayName: "User country", userClaims: new List<string> { "country" })
    };

    static IEnumerable<Client> GetClients() => new List<Client>
    {
       /* new Client
        {
            ClientName = "Blazor WASM",
            ClientId = "BlazorWASM",
            AllowedGrantTypes = GrantTypes.Hybrid,
            RedirectUris = new List<string>{ "https://localhost:9001/signin-oidc" },
            RequirePkce = false,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Address,
                "roles",
                "BlazorAPI",
                "country"
            },
            ClientSecrets = { new Secret("BlazorWASMSecret".Sha512()) },
            RequireConsent = true
        },*/
        new Client
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
        /*        IdentityServerConstants.StandardScopes.Address,
                "roles",
                "BlazorAPI",
                "country"*/
            }
            //ClientSecrets = { new Secret("BlazorWASMSecret".Sha512()) },
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

    internal static IServiceCollection ConfigureIdentityServices(this IServiceCollection services)
    {
        services.AddIdentityServer()
        .AddInMemoryApiScopes(Config.GetApiScopes())
        .AddInMemoryApiResources(Config.GetApiResources())
        .AddInMemoryIdentityResources(Config.GetIdentityResources())
        .AddTestUsers(Config.GetUsers())
        .AddInMemoryClients(Config.GetClients())
        .AddDeveloperSigningCredential();
        services.AddControllersWithViews();

        return services;
    }
}
