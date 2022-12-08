using Blazor.Shared.Entities.Constants;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public sealed class ImageSizeValidationFilter : IActionFilter
{
    private readonly Serilog.ILogger _logger;

    public ImageSizeValidationFilter(Serilog.ILogger logger)
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

        if (image.ImageFile.Length is >= Constants.MaximalImageSize or <= Constants.MinimalImageSize)
        {
            _logger.Warning("File {FileName}'s MIMO type doesn't match any file extensions. (Err: {ErrorCode})",
                image.FileName,
                (int)ErrorCodes.FileLengthOutOfBoundsError);

            context.Result = new UnprocessableEntityObjectResult(String.Format("File {0}'s length must be ranging from {1} B to {2} B (Err: {3})",
                image.FileName,
                Constants.MinimalImageSize,
                Constants.MaximalImageSize,
                (int)ErrorCodes.FileLengthOutOfBoundsError));

            return;
        }
    }
}
