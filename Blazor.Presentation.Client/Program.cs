using Blazor.Presentation.Client;
using Blazor.Presentation.Client.Services;
using Blazor.Shared.Entities.Identity;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzczMDAwQDMyMzAyZTMzMmUzMFBsY1JXeHQvejRFL0d0VFZmT2FqZzRUVHFJRzBaWTJOQzQvd25HbWE3cEE9");
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient<CarouselItemService>(client => client.BaseAddress = new Uri("https://localhost:11001/"))
    .AddHttpMessageHandler(handlerConfig =>
    {
        AuthorizationMessageHandler handler = handlerConfig.GetService<AuthorizationMessageHandler>().ConfigureHandler(
            authorizedUrls: new[] { "https://localhost:11001" },
            scopes: new[] { "BlazorAPI" }
        );

        return handler;
    });
builder.Services.AddHttpClient("Default", client => client.BaseAddress = new Uri("https://localhost:11001/api/"))
    .AddHttpMessageHandler(handlerConfig =>
    {
        AuthorizationMessageHandler handler = handlerConfig.GetService<AuthorizationMessageHandler>().ConfigureHandler(
            authorizedUrls: new[] { "https://localhost:11001" },
            scopes: new[] { "BlazorAPI" }
        );

        return handler;
    });

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(Policies.FromCzechRepublic, Policies.FromCzechRepublicPolicy());
    options.AddPolicy(Policies.FromFrance, Policies.FromFrancePolicy());
});

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("oidc", options.ProviderOptions);
    options.UserOptions.RoleClaim = "role";
});

builder.Services.AddSingleton<CarouselItemService>();
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();
