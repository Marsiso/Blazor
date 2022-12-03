using Blazor.Presentation.Server.Extensions;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Entities.Mappings;
using Microsoft.AspNetCore.HttpOverrides;
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
});

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureSqlContext(builder.Configuration)
    .ConfigureCors()
    .ConfigureIISIntegration()
    .ConfigureRepositoryManager()
    .AddAutoMapper(typeof(CarouselItemMappingProfile), typeof(ImageMappingProfile))
    .AddScoped<CarouselItemExistsValidationFilter>()
    .AddScoped<ImageExistsValidationFilter>();

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
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoint =>
    {
        endpoint.MapControllers();
    });


logger.Information("Project services have been configured");

app.Run();
logger.Dispose();