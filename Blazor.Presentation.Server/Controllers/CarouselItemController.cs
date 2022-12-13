using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Presentation.Server.ModelBinders;
using Blazor.Presentation.Server.Utility;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blazor.Presentation.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class CarouselItemController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly CarouselItemLinks _carouselItemLinks;

    public CarouselItemController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper, CarouselItemLinks carouselItemLinks)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _carouselItemLinks = carouselItemLinks;
    }

    [HttpOptions(Name = "GetCarouselItemsOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCarouselItemsOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE, PUT");

        return Ok();
    }

    [HttpGet(Name = "GetAllCarouselItemsAsync")]
    [HttpHead(Name = "GetAllCarouselItemsHeadAsync")]
    //[Authorize(Policy = Policies.FromCzechia)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetAllCarouselItemsAsync([FromQuery] CarouselItemParameters carouselItemParameters)
    {
        var carouselItemEntities = await _repository.CarouselItem
            .GetAllCarouselItemsAsync(carouselItemParameters, false);
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(carouselItemEntities.MetaData));
        var carouselItemsDto = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItemEntities);

        var links = _carouselItemLinks.TryGenerateLinks(
            carouselItemsDto, 
            carouselItemParameters.Fields, 
            HttpContext);

        return links.HasLinks
            ? Ok(links.LinkedEntities)
            : Ok(links.ShapedEntities);
    }

    [HttpGet("Collection/({carouselItemIds})", Name = "GetCarouselItemsByIdsAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCarouselItemsByIdsAsync([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> carouselItemIds)
    {
        if (carouselItemIds == null)
        {
            _logger.Warning("Parameter ids sent from client is null");
            return BadRequest("Parameter ids sent from client is null");
        }

        var carouselItemEntities = await _repository.CarouselItem.GetCarouselItemsByIds(carouselItemIds, false);
        if (carouselItemIds.Count() != carouselItemEntities.Count())
        {
            _logger.Warning("Some ids are not valid in a collection");
            return NotFound("Some ids are not valid in a collection");
        }

        var carouselItemsToReturn = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItemEntities);
        return Ok(carouselItemsToReturn);
    }

    [HttpGet("{carouselItemId:int}", Name = "GetCarouselItem")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute), Order = 1)]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCarouselItem(int carouselItemId, [FromQuery] CarouselItemParameters carouselItemParameters)
    {
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        var carouselItemDto = _mapper.Map<CarouselItemDto>(carouselItemEntity);
        
        var links = _carouselItemLinks.TryGenerateLinks(
            new []{ carouselItemDto }, 
            carouselItemParameters.Fields, 
            HttpContext);

        return links.HasLinks
            ? Ok(links.LinkedEntities)
            : Ok(links.ShapedEntities);
    }

    [HttpPost("Collection", Name = "CreateCarouselItemCollectionAsync")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCarouselItemCollectionAsync([FromBody] IEnumerable<CarouselItemForCreationDto> carouselItemCollection)
    {
        if (carouselItemCollection == null)
        {
            _logger.Warning("CarouselItem collection sent from client is null.");
            return BadRequest("CarouselItem collection is null");
        }

        var carouselItemEntities = _mapper.Map<IEnumerable<CarouselItemEntity>>(carouselItemCollection);
        foreach (var carouselItemEntity in carouselItemEntities)
        {
            if (!TryValidateModel(carouselItemEntity))
            {
                _logger.Error("Invalid model state for the carousel item"); 
                return UnprocessableEntity(ModelState);
            }
            
            _repository.CarouselItem.CreateCarouselItem(carouselItemEntity);
        }

        await _repository.SaveAsync();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CarouselItemDto>>(carouselItemEntities);
        var ids = string.Join(",", companyCollectionToReturn.Select(ci => ci.Id));

        return CreatedAtRoute(nameof(GetCarouselItemsByIdsAsync), new { carouselItemIds = ids }, companyCollectionToReturn);
    }

    [HttpPost(Name = "CreateCarouselItemAsync")]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCarouselItemAsync([FromBody] CarouselItemForCreationDto carouselItem)
    {
        var carouselItemEntity = _mapper.Map<CarouselItemEntity>(carouselItem);
        _repository.CarouselItem.CreateCarouselItem(carouselItemEntity);
        await _repository.SaveAsync();

        var carouselItemToReturn = _mapper.Map<CarouselItemDto>(carouselItemEntity);

        return CreatedAtRoute(nameof(GetCarouselItem), new { carouselItemId = carouselItemToReturn.Id }, carouselItemToReturn);
    }

    [HttpDelete("{carouselItemId:int}", Name = "DeleteCarouselItem")]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCarouselItemAsync(int carouselItemId)
    {
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        _repository.CarouselItem.DeleteCarouselItem(carouselItemEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Carousel item has been successfully deleted");
        return NoContent();
    }

    [HttpPut("{carouselItemId:int}", Name = "UpdateCarouselItemAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCarouselItemAsync(int carouselItemId, [FromBody] CarouselItemForUpdateDto carouselItem)
    {
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        _mapper.Map(carouselItem, carouselItemEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Carousel item has been successfully updated");
        return NoContent();
    }
    
    
    [HttpPatch("{carouselItemId:int}", Name = "PartiallyUpdateCarouselItemAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PartiallyUpdateCarouselItemAsync(int carouselItemId, [FromBody] JsonPatchDocument<CarouselItemForUpdateDto> patchDoc)
    {
        var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
        var carouselItemToPatch = _mapper.Map<CarouselItemForUpdateDto>(carouselItemEntity);
        
        patchDoc.ApplyTo(carouselItemToPatch);
        if (!TryValidateModel(carouselItemToPatch))
        {
            _logger.Error("Invalid model state for the patch document"); 
            return UnprocessableEntity(ModelState);
        }

        _mapper.Map(carouselItemToPatch, carouselItemEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Carousel item has been successfully updated");
        return NoContent();
    }
}
