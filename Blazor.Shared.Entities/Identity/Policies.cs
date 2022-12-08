using Microsoft.AspNetCore.Authorization;

namespace Blazor.Shared.Entities.Identity;

public static class Policies
{
    public static readonly string FromCzechia = "FromCzechia";
    public static readonly string FromSlovakia = "FromSlovakia";
    public static readonly string FromGermany = "FromGermany";
    public static readonly string FromFrance = "FromFrance";

    public static AuthorizationPolicy FromCzechiaPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Czechia")
        .Build();

    public static AuthorizationPolicy FromSlovakiaPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Slovakia")
        .Build();

    public static AuthorizationPolicy FromGermanyPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Germany")
        .Build();

    public static AuthorizationPolicy FromFrancePolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "France")
        .Build();
}
