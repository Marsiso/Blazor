using Blazor.Presentation.Server.Identity.Extensions;
using Blazor.Presentation.Server.Identity.Identity;
using Blazor.Presentation.Server.Identity.Validators;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Implementations.Repositories;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddTransient<IResourceOwnerPasswordValidator, ApplicationResourceOwnerPasswordValidator>(); // Password validator
builder.Services.ConfigureIdentityServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseCors("Default");
app.UseRouting();
app.InitializeIdentityDatabase();
app.UseIdentityServer();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
