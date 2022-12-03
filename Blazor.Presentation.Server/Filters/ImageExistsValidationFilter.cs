using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blazor.Presentation.Server.Filters;

public sealed class ImageExistsValidationFilter : IAsyncActionFilter
{
    private readonly IRepositoryManager _repository;

    private readonly Serilog.ILogger _logger;

    public ImageExistsValidationFilter(IRepositoryManager repository, Serilog.ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var trackChanges = method.Equals("PUT") || method.Equals("PATCH") || method.Equals("POST");
        var carouselItemEntity = context.HttpContext.Items["carouselItemEntity"] as CarouselItemEntity;

        var imageEntity = await _repository.Image.GetImageByCarouselItemAsync(carouselItemEntity, trackChanges);
        if (imageEntity == null)
        {
            _logger.Information("Carousel item with id: {Id} doesn't have image in the database", carouselItemEntity.Id);
            context.Result = new NotFoundResult();
        }
        else
        {
            context.HttpContext.Items.Add("imageEntity", imageEntity);
            await next();
        }
    }
}
