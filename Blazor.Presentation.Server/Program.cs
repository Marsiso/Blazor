using AspNetCoreRateLimit;
using Blazor.Presentation.Server.Extensions;
using Blazor.Presentation.Server.Filters;
using Blazor.Presentation.Server.Services;
using Blazor.Presentation.Server.Utility;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Identity;
using Blazor.Shared.Entities.Mappings;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.DataShaping;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(logger);

logger.Information("Project build has been initiated");

builder.Services.AddControllers(configure =>
{
    configure.RespectBrowserAcceptHeader = true;
    configure.ReturnHttpNotAcceptable = true;
    configure.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
})
    .AddNewtonsoftJson()
    //.AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormatter();

builder.Services
    .Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddEndpointsApiExplorer()
    .ConfigureSwagger()
    .ConfigureSqlContext(builder.Configuration)
    .ConfigureCors()
    .ConfigureIISIntegration()
    .ConfigureRepositoryManager()
    .AddAutoMapper(typeof(CarouselItemMappingProfile), typeof(ImageMappingProfile), typeof(ProductMappingProfile),
        typeof(OrderItemMappingProfile), typeof(OrderMappingProfile), typeof(UserMappingProfile), typeof(ResetPasswordRequestMappingProfile))
    .AddScoped<CarouselItemExistsValidationFilter>()
    .AddScoped<ProductExistsValidationFilter>()
    .AddScoped<OrderItemExistsValidationFilter>()
    .AddScoped<OrderExistsValidationFilter>()
    .AddScoped<UserExistsValidationFilter>()
    .AddScoped<ValidationFilter>()
    .AddScoped<ImageExistsValidationFilter>()
    .AddScoped<ImageFormatValidationFilter>()
    .AddScoped<ImageSizeValidationFilter>()
    .AddScoped<IDataShaper<CarouselItemDto>, DataShaper<CarouselItemDto>>()
    .AddSingleton<EmailBrokerService>()
    .AddCustomMediaTypes()
    .AddScoped<ValidateMediaTypeAttribute>()
    .AddScoped<CarouselItemLinks>()
    .ConfigureVersioning()
    .ConfigureResponseCaching()
    .ConfigureHttpCacheHeaders()
    .AddMemoryCache()
    .ConfigureRateLimitingOptions()
    .AddHttpContextAccessor()
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.RequireHttpsMetadata = false; // for development purposes, disable in production!;
        opt.Authority = "https://localhost:9001";
        opt.Audience = "BlazorAPI";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.FromCzechRepublic, Policies.FromCzechRepublicPolicy());
});

logger.Information("Services have been registered");

var app = builder.Build();

logger.Information("Project was successfully built");

if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app
    .ConfigureExceptionHandler(logger);

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseCors("Default")
    .UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    })
    .UseResponseCaching()
    .UseHttpCacheHeaders()
    .UseIpRateLimiting()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoint => endpoint.MapControllers());

logger.Information("Project services have been configured");

app.Run();
logger.Dispose();
