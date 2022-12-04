﻿using Microsoft.AspNetCore.Authorization;

namespace Blazor.Presentation.Server.Extensions;

public static class IdentityExtensions
{
    public const string FromCzechia = "FromCzechia";
    public static AuthorizationPolicy FromCzechiaPolicy() => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("country", "Czechia")
        .Build();
}
