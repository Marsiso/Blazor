using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Presentation.Server.Controllers;

[Route("api/CarouselItem/{carouselItemId:int}/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    readonly Serilog.ILogger _logger;
    readonly IRepositoryManager _repository;
    readonly IMapper _mapper;

    public ImageController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }


    [HttpGet(Name = "GetImageAsync")]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter))]
    public async Task<IActionResult> GetImageAsync(int carouselItemId)
    {
        var carouselItemEntity = HttpContext.Items["carouselItemEntity"] as CarouselItemEntity;
        var imageEntity = await _repository.Image.GetImageByCarouselItemAsync(carouselItemEntity, false);
        if (imageEntity == null)
        {
            _logger.Information("Carousel item with id: {Id} doesn't have image in the database", carouselItemId);
            return NotFound();
        }

        var imageDto = _mapper.Map<ImageDto>(imageEntity);
        return Ok(imageDto);
    }
}
