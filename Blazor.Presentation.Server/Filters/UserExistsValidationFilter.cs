using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Filters;

public sealed class UserExistsValidationFilter : IAsyncActionFilter
{
    private readonly IRepositoryManager _repository;

    private readonly Serilog.ILogger _logger;

    public UserExistsValidationFilter(IRepositoryManager repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var trackChanges = method.Equals("PUT") || method.Equals("PATCH") || method.Equals("POST");
        var userEmail = context.ActionArguments["userEmail"] as UserEmailDto;

        if (String.IsNullOrEmpty(userEmail.Email))
        {
            _logger.Warning("Email invalid format");
            context.Result = new UnprocessableEntityObjectResult("Email invalid format");
            return;
        }

        var userEntity = await _repository.User.GetUserAsync(userEmail.Email, trackChanges);
        if (userEntity == null)
        {
            _logger.Warning("User with email: {Email} doesn't exist in the database", userEmail);
            context.Result = new NotFoundObjectResult(String.Format("User with email: {0} doesn't exist in the database", userEmail));
            return;
        }

        context.HttpContext.Items.Add(nameof(UserEntity), userEntity);

        await next();
    }
}