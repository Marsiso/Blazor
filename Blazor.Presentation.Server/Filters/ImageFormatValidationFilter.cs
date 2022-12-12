using Blazor.Presentation.Server.Utility;
using Blazor.Shared.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public sealed class ImageFormatValidationFilter : IActionFilter
{
    private readonly Serilog.ILogger _logger;

    public ImageFormatValidationFilter(Serilog.ILogger logger)
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
        if (context.ActionArguments.SingleOrDefault(p => p.Value.ToString().Contains(nameof(ImageForCreationDto))).Value is not ImageForCreationDto image)
        {
            _logger.Warning("Object cast failure. Controller: {Controller} Action: {Action}", controller, action);

            context.Result = new UnprocessableEntityObjectResult(String.Format("Object cast failure. Controller: {0} Action: {1}", controller, action));

            return;
        }

        if (!FileExtensionHelper.TryGetFileExtension(image.ImageFile.ContentType, out var extension))
        {
            _logger.Warning("Unsupported media type. Controller: {Controller} Action: {Action}", controller, action);
            context.Result = new UnprocessableEntityObjectResult(String.Format("Unsupported media type. Controller: {0} Action: {1}", controller, action));

            return;
        }

        context.HttpContext.Items.Add("extension", extension);
    }
}
