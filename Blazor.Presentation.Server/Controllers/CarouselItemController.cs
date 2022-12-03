using AutoMapper;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Presentation.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CarouselItemController : ControllerBase
{
    readonly ILogger<CarouselItemController> _logger;
    readonly IRepositoryManager _repository;
    readonly IMapper _mapper;

    public CarouselItemController(ILogger<CarouselItemController> logger, IRepositoryManager repository, IMapper mapper)
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
}
