using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public class OrderItemController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public OrderItemController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpOptions(Name = "GetOrderItemOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOrderItemOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE, PUT");

        return Ok();
    }
    
    [HttpGet(Name = "GetAllOrderItemsAsync")]
    [HttpHead(Name = "GetAllOrderItemsAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAllOrderItemsAsync([FromQuery] OrderItemParameters orderItemParameters)
    {
        var orderItemEntities = await _repository.OrderItem
            .GetAllOrderItemsAsync(orderItemParameters, false);
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(orderItemEntities.MetaData));
        var orderItemEntitiesDto = _mapper.Map<IEnumerable<OrderItemDto>>(orderItemEntities);
        
        return Ok(orderItemEntitiesDto);
    }
    
    [HttpGet("{orderItemId}", Name = "GetOrderItemAsync")]
    [HttpHead("{orderItemId}", Name = "GetOrderItemAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetOrderItemAsync(int orderItemId)
    {
        var orderItemEntity = await _repository.OrderItem
            .GetOrderItemAsync(orderItemId, false);
        var productDto = _mapper.Map<OrderItemDto>(orderItemEntity);
        
        return Ok(productDto);
    }
    
    [HttpPost(Name = "CreateOrderItemAsync")]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrderItemAsync([FromBody] OrderItemForCreationDto orderItem)
    {
        var orderItemEntity = _mapper.Map<OrderItemEntity>(orderItem);
        _repository.OrderItem.CreateOrderItem(orderItemEntity);
        await _repository.SaveAsync();

        var orderItemDto = _mapper.Map<OrderItemDto>(orderItemEntity);

        return CreatedAtRoute(nameof(GetOrderItemAsync), new { orderItemId = orderItemDto.Id }, orderItemDto);
    }
    
    [HttpDelete("{orderItemId:int}", Name = "DeleteOrderItem")]
    [ServiceFilter(typeof(OrderItemExistsValidationFilter))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrderItemAsync(int orderItemId)
    {
        var orderItemEntity = HttpContext.Items[nameof(OrderItemEntity)] as OrderItemEntity;
        _repository.OrderItem.DeleteOrderItem(orderItemEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Order item has been successfully deleted");
        return NoContent();
    }
    
    [HttpPut("{orderItemId:int}", Name = "UpdateOrderItemAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(OrderItemExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderItemAsync(int orderItemId, [FromBody] OrderItemForUpdateDto orderItem)
    {
        var orderItemEntity = HttpContext.Items[nameof(OrderItemEntity)] as OrderItemEntity;
        _mapper.Map(orderItem, orderItemEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Order item has been successfully updated");
        return NoContent();
    }
}