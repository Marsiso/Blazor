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
public class OrderController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public OrderController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpOptions(Name = "GetOrderOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOrderOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE, PUT");

        return Ok();
    }
    
    [HttpGet(Name = "GetAllOrdersAsync")]
    [HttpHead(Name = "GetAllOrdersAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAllOrdersAsync([FromQuery] OrderParameters orderParameters)
    {
        var orderEntities = await _repository.Order
            .GetAllOrdersAsync(orderParameters, false);
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(orderEntities.MetaData));
        var orderEntitiesDto = _mapper.Map<IEnumerable<OrderDto>>(orderEntities);
        
        return Ok(orderEntitiesDto);
    }
    
    [HttpGet("{orderId}", Name = "GetOrderAsync")]
    [HttpHead("{orderId}", Name = "GetOrderAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetOrderAsync(int orderId)
    {
        var orderEntity = await _repository.Order
            .GetOrderAsync(orderId, false);
        var orderDto = _mapper.Map<OrderDto>(orderEntity);
        
        return Ok(orderDto);
    }
    
    [HttpPost(Name = "CreateOrderAsync")]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderForCreationDto order)
    {
        var orderEntity = _mapper.Map<OrderEntity>(order);
        _repository.Order.CreateOrder(orderEntity);
        await _repository.SaveAsync();

        var orderDto = _mapper.Map<OrderDto>(orderEntity);

        return CreatedAtRoute(nameof(GetOrderAsync), new { orderId = orderDto.Id }, orderDto);
    }

    [HttpPut("{orderId:int}", Name = "UpdateOrderAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(OrderExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderAsync(int orderId, [FromBody] OrderForUpdateDto order)
    {
        var orderEntity = HttpContext.Items[nameof(OrderEntity)] as OrderEntity;
        _mapper.Map(order, orderEntity);
        await _repository.SaveAsync();
        
        return NoContent();
    }

    [HttpDelete("{orderId:int}", Name = "DeleteOrder")]
    [ServiceFilter(typeof(OrderExistsValidationFilter))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrderAsync(int orderId)
    {
        var orderEntity = HttpContext.Items[nameof(OrderEntity)] as OrderEntity;
        _repository.Order.DeleteOrder(orderEntity);
        await _repository.SaveAsync();
        
        return NoContent();
    }
}