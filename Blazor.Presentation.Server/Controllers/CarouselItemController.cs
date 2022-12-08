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
    //[Authorize(Policy = Policies.FromCzechia)]
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
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        var carouselItemDto = _mapper.Map<CarouselItemDto>(carouselItemEntity);
        return Ok(carouselItemDto);
    }

    [HttpPost(Name = "CreateCarouselItemAsync")]
    public async Task<IActionResult> CreateCarouselItemAsync([FromBody] CarouselItemForCreationDto carouselItem)
    {
        if (carouselItem == null)
        {
            _logger.Warning("CarouselItemForCreationDto object sent from client is null");
            return BadRequest("CarouselItemForCreationDto object sent from client is null");
        }

        var carouselItemEntity = _mapper.Map<CarouselItemEntity>(carouselItem);
        _repository.CarouselItem.CreateCarouselItem(carouselItemEntity);
        await _repository.SaveAsync();

        var carouselItemToReturn = _mapper.Map<CarouselItemDto>(carouselItemEntity);

        return CreatedAtRoute(/*nameof(GetCarouselItemAsync)*/"CreateCarouselItemAsync", new { id = carouselItemToReturn.Id }, carouselItemToReturn);
    }

    [HttpDelete("{carouselItemId:int}", Name = "DeleteCarouselItem")]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter))]
    public async Task<IActionResult> DeleteCarouselItemAsync(int carouselItemId)
    {
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        _repository.CarouselItem.DeleteCarouselItem(carouselItemEntity);
        await _repository.SaveAsync();

        return NotFound();
    }
}
