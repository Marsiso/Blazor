using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Presentation.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CarouselItemController : ControllerBase
{
    readonly Serilog.ILogger _logger;
    readonly IRepositoryManager _repository;
    readonly IMapper _mapper;

    public CarouselItemController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetAllCarouselItemsAsync")]
    public async Task<IActionResult> GetAllCarouselItemsAsync()
    {
        var carouselItemEntities = await _repository.CarouselItem.GetAllCarouselItemsAsync(false);
        var carouselItemsDtos = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItemEntities);
        return Ok(carouselItemsDtos);
    }

    [HttpGet("{carouselItemId:int}", Name = "GetCarouselItemAsync")]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter))]
    public async Task<IActionResult> GetCarouselItemAsync(int carouselItemId)
    {
        var carouselItemEntity = HttpContext.Items["carouselItemEntity"] as CarouselItemEntity;
        var carouselItemDto = _mapper.Map<CarouselItemDto>(carouselItemEntity);
        return Ok(carouselItemDto);
    }
}
