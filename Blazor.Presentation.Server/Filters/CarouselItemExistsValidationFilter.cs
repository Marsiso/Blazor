using Blazor.Shared.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public class CarouselItemExistsValidationFilter : IAsyncActionFilter
{
    private readonly IRepositoryManager _repository;

    private readonly Serilog.ILogger _logger;

    public CarouselItemExistsValidationFilter(IRepositoryManager repository, Serilog.ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var trackChanges = method.Equals("PUT") || method.Equals("PATCH") || method.Equals("POST");
        var id = (int)context.ActionArguments["carouselItemId"]!;

        var carouselItemEntity = await _repository.CarouselItem.GetCarouselItemAsync(id, trackChanges);
        if (carouselItemEntity == null)
        {
            _logger.Warning("Carousel item with id: {Id} doesn't exist in the database", id);
            context.Result = new NotFoundResult();
        }
        else
        {
            context.HttpContext.Items.Add("carouselItemEntity", carouselItemEntity);
            await next();
        }
    }
}
