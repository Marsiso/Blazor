using Microsoft.AspNetCore.Authorization;

namespace Blazor.Presentation.Client.Identity;

public static class Policies
{
    public const string FromCzechia = "FromCzechia";
    public static AuthorizationPolicy FromCzechiaPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Czechia")
        .Build();
}
