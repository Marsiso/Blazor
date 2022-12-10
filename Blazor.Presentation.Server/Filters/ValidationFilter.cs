using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public sealed class ValidationFilter : IActionFilter
{
    private readonly Serilog.ILogger _logger;

    public ValidationFilter(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var param = context.ActionArguments
            .SingleOrDefault(p => p.Value.ToString().Contains("Dto")).Value;

        if (param == null)
        {
            _logger.Warning("Object sent from client is null. Controller: {Controller} Action: {Action}", controller, action);
            context.Result = new BadRequestObjectResult(String.Format("Object sent from client is null. Controller: {0}, action: {1}", controller, action));

            return;
        }

        if (context.ModelState.IsValid) return;
        _logger.Warning("Invalid model state for the object. Controller: {Controller} Action: {Action}", controller, action);
        context.Result = new UnprocessableEntityObjectResult(context.ModelState);
    }
}
