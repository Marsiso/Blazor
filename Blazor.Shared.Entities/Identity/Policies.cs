using Microsoft.AspNetCore.Authorization;

namespace Blazor.Shared.Entities.Identity;

public static class Policies
{
    public const string FromCzechRepublic = "FromCzechRepublic";
    public const string FromSlovakia = "FromSlovakia";
    public const string FromGermany = "FromGermany";
    public const string FromFrance = "FromFrance";

    public static AuthorizationPolicy FromCzechRepublicPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Czech Republic")
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
