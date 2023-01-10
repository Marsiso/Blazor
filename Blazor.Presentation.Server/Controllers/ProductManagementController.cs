using System.Dynamic;
using AutoMapper;
using Blazor.Presentation.Server.Utility;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public sealed class ProductManagementController : ControllerBase
{

    [HttpGet(Name = "DownloadBatchAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> DownloadBatchAsync(
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] SqlContext sqlContext)
    {
        var productModels = await (from p in sqlContext.Products
            join ci in sqlContext.CarouselItems on p.CarouselItemId equals ci.Id
            join i in sqlContext.Images on ci.Id equals i.CarouselItemId
            select new
            {
                ProductIdentifier = p.Id,
                ProductName = p.Name,
                ProductPrice = p.Price,
                ImageAlt = ci.Alt,
                ImageCaption = ci.Caption,
                FileUnsafeName = i.UnsafeName,
                FileSafeName = i.SafeName
            }).ToArrayAsync();

        var len = productModels.Length;
        var array = new ExpandoObject[len];
        Parallel.For(0, len, (index) =>
        {
            var imageTask = ImageDownloadService.DownloadImage(Path.Combine(webHost.WebRootPath, "images", "carousel", productModels[index].FileSafeName));
            dynamic product = new ExpandoObject();
            
            product.ProductIdentifier = productModels[index].ProductIdentifier;
            product.ProductName = productModels[index].ProductName;
            product.ProductPrice = productModels[index].ProductPrice;
            product.ImageAlt = productModels[index].ImageAlt;
            product.ImageCaption = productModels[index].ImageCaption;
            
            imageTask.Wait();        
            product.ImageSource = imageTask.Result;

            array[index] = product;
        });

        return Ok(array);
    }
    
    [HttpGet("{id}", Name = "DownloadAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> DownloadAsync(
        int id,
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] SqlContext sqlContext)
    {
        var productModel = await (from p in sqlContext.Products
            join ci in sqlContext.CarouselItems on p.CarouselItemId equals ci.Id
            join i in sqlContext.Images on ci.Id equals i.CarouselItemId
            where p.Id == id
            select new
            {
                ProductIdentifier = p.Id,
                ProductName = p.Name,
                ProductPrice = p.Price,
                ImageAlt = ci.Alt,
                ImageCaption = ci.Caption,
                FileUnsafeName = i.UnsafeName,
                FileSafeName = i.SafeName
            }).SingleOrDefaultAsync();

        var imageTask = ImageDownloadService.DownloadImage(Path.Combine(webHost.WebRootPath, "images", "carousel", productModel.FileSafeName));
        dynamic product = new ExpandoObject();
            
        product.ProductIdentifier = productModel.ProductIdentifier;
        product.ProductName = productModel.ProductName;
        product.ProductPrice = productModel.ProductPrice;
        product.ImageAlt = productModel.ImageAlt;
        product.ImageCaption = productModel.ImageCaption;
            
        imageTask.Wait();        
        product.ImageSource = imageTask.Result;
        
        return Ok(product);
    }

    [HttpPost(Name = "UploadAsync")]
    //[ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadAsync(
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] IMapper mapper,
        [FromServices] IRepositoryManager repository,
        [FromBody] ProductForCreationDto product)
    {
        if (product is null)
        {
            return BadRequest(new { ErrorMessage = "Product to be listed can not be an empty object" });
        }

        var carouselItem = product.CarouselItem;
        if (carouselItem is null)
        {
            return BadRequest(new { ErrorMessage = "Carousel item to be listed can not be an empty object" });
        }

        var image = product.CarouselItem.Image;
        if (image?.ImageFile is null)
        {
            return BadRequest(new { ErrorMessage = "Image to be uploaded can not be an empty object" });
        }

        var productEntity = mapper.Map<ProductEntity>(product);


        if (!FileExtensionHelper.TryGetFileExtension(image.ImageFile.ContentType, out var extension))
        {
            return UnprocessableEntity("Image does not match any known extensions");
        }

        var (unsafeName, safeName) = await ImageUploadService.UploadNewImage(image.ImageFile, webHost.WebRootPath, extension);
        if (string.IsNullOrEmpty(unsafeName) || string.IsNullOrEmpty(safeName))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Image on upload internal server error");
        }

        productEntity.CarouselItem.Image.UnsafeName = unsafeName;
        productEntity.CarouselItem.Image.SafeName = safeName;
            
        repository.Product.CreateProduct(productEntity);
        await repository.SaveAsync();
            
        var productToReturn = mapper.Map<ProductDto>(productEntity);
        
        return CreatedAtRoute(nameof(DownloadAsync), new { productId = productEntity.Id }, productToReturn);
    }
}