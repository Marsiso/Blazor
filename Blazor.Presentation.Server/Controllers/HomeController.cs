using System.Text;
using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public sealed class HomeController : ControllerBase
{
    [HttpOptions(Name = "GetHomeOptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetHomeOptions()
    {
        Response.Headers.Add("Allow", "GET, POST");

        return Ok();
    }

    [HttpGet("Cart", Name = nameof(GetAllPricedProductsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAllPricedProductsAsync(
        [FromServices] SqlContext sqlContext,
        [FromServices] IWebHostEnvironment webHost)
    {
        var products = await (from p in sqlContext.Products
            join ci in sqlContext.CarouselItems on p.CarouselItemId equals ci.Id
            join i in sqlContext.Images on ci.Image.Id equals i.Id
            select new
            {
                p.Id,
                p.Name,
                p.Price,
                ci.Alt,
                ci.Caption,
                i.SafeName,
                i.UnsafeName,
                Src = String.Empty
            }).ToArrayAsync(); // Skip() and Take() for paging

        var basePath = webHost.WebRootPath;
        IList<ProcessedProduct> processedProducts = new List<ProcessedProduct>();
        for (var index = 0; index < products.Length; index++)
        {
            var src = String.Empty;
            try
            {
                var path = Path.Combine(basePath, "images", "carousel", products[index].SafeName);
                if (System.IO.File.Exists(path))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(path);
                    var payload = Convert.ToBase64String(bytes);
                    var result = new FileExtensionContentTypeProvider()
                        .TryGetContentType(products[index].SafeName, out var contentType);

                    if (result)
                    {
                        var strBuilder = new StringBuilder();
                        strBuilder.Append("data:").Append(contentType).Append(";base64,").Append(payload);
                        src = strBuilder.ToString();
                    }
                }
            }
            finally
            {
                processedProducts.Add(new ProcessedProduct(
                    products[index].Id,
                    products[index].Name,
                    products[index].Price,
                    products[index].Alt,
                    products[index].Caption,
                    src,
                    0));
            }
        }
        
        return Ok(processedProducts);
    }
    
    [HttpPost("Cart", Name = nameof(ProcessShoppingCartAsync))]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessShoppingCartAsync(
        [FromServices] IRepositoryManager repository,
        [FromServices] IMapper autoMapper,
        [FromServices] IWebHostEnvironment webHost,
        [FromBody] OrderForCreationDto order)
    {
        var orderEntity = autoMapper.Map<OrderEntity>(order);
        orderEntity.DateTimeCreated = DateTime.Now;
        repository.Order.CreateOrder(orderEntity);
        await repository.SaveAsync();
        
        return Ok();
    }
}