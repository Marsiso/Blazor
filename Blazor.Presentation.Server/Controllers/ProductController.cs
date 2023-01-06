using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public sealed class ProductController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public ProductController(ILogger logger, IRepositoryManager repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpOptions(Name = "GetProductsOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetProductsOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, DELETE, PUT");

        return Ok();
    }
    
    [HttpGet(Name = "GetAllProductsAsync")]
    [HttpHead(Name = "GetAllProductsAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAllCarouselItemsAsync([FromQuery] ProductParameters productParameters)
    {
        var productEntities = await _repository.Product
            .GetAllProductsAsync(productParameters, false);
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(productEntities.MetaData));
        var productEntitiesDto = _mapper.Map<IEnumerable<ProductDto>>(productEntities);
        
        return Ok(productEntitiesDto);
    }
    
    [HttpGet("{productId}", Name = "GetProductAsync")]
    [HttpHead("{productId}", Name = "GetProductAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetProductAsync(int productId)
    {
        var productEntity = await _repository.Product
            .GetProductAsync(productId, false);
        var productDto = _mapper.Map<ProductDto>(productEntity);
        
        return Ok(productDto);
    }
    
    [HttpPost(Name = "CreateProductAsync")]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductForCreationDto product)
    {
        var productEntity = _mapper.Map<ProductEntity>(product);
        _repository.Product.CreateProduct(productEntity);
        await _repository.SaveAsync();

        var productToReturn = _mapper.Map<CarouselItemDto>(productEntity);

        return CreatedAtRoute(nameof(GetProductAsync), new { productId = productToReturn.Id }, productToReturn);
    }
    
    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ServiceFilter(typeof(ProductExistsValidationFilter))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProductAsync(int productId)
    {
        var productEntity = HttpContext.Items[nameof(ProductEntity)] as ProductEntity;
        _repository.Product.DeleteProduct(productEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Product has been successfully deleted");
        return NoContent();
    }
    
    [HttpPut("{productId:int}", Name = "UpdateProductAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(ProductExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProductAsync(int productId, [FromBody] ProductForUpdateDto product)
    {
        var productEntity = HttpContext.Items[nameof(ProductEntity)] as ProductEntity;
        _mapper.Map(product, productEntity);
        await _repository.SaveAsync();
        
        _logger.Information("Product has been successfully updated");
        return NoContent();
    }
}