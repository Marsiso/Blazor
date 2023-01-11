using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Net;
using Blazor.Presentation.Server.Utility;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using Constants = Blazor.Shared.Entities.Constants.Constants;
using ILogger = Serilog.ILogger;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/[controller]")]
[ApiController]
public sealed class ProductManagementController : ControllerBase
{

    [HttpGet(Name = "GetAllAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAllAsync(
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
            var imageTask = ImageFileHandler.DownloadImage(Path.Combine(webHost.WebRootPath, "images", "carousel", productModels[index].FileSafeName));
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
    
    [HttpGet("{id}", Name = "GetAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public async Task<IActionResult> GetAsync(
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

        if (productModel is null)
        {
            return NotFound(new { ErrorMessage = $"Product with an ID: {id} does not exists within the database" });
        }

        var imageTask = ImageFileHandler.DownloadImage(Path.Combine(webHost.WebRootPath, "images", "carousel", productModel.FileSafeName));
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

    [HttpPost(Name = "CreateAsync")]
    //[ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] SqlContext sqlContext,
        [FromServices] ILogger logger,
        [FromForm] ProductModelForUpdateDto product)
    {
        // Validate message content
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(product);
        var validationResults = new List<ValidationResult>();
        if (Validator.TryValidateObject(product, context, validationResults, true) is false)
        {
            return UnprocessableEntity(new { ErrorMessage = "Product object validation encountered problems, errors: " + string.Join(", ", validationResults) });
        }

        // Check if image is included in the message content
        if (product.ImageFile is null)
        {
            return UnprocessableEntity(new { ErrorMessage = "Product image object can not be null" });
        }

        // Check if image corresponds to the file size limitations
        if (product.ImageFile.Length is >= Constants.MaximalImageSize or <= Constants.MinimalImageSize)
        {
            return UnprocessableEntity(new
            {
                ErrorMessage =
                    $"Invalid image file length, minimal file length is {Constants.MinimalImageSize} B and maximal {Constants.MaximalImageSize}"
            });
        }

        try
        {
            // Try get file extensions from mime types
            if (FileExtensionHelper.TryGetFileExtension(product.ImageFile.ContentType, out var extension))
            {
                // Generate file names
                var safeName = Path.GetRandomFileName() + (extension.Contains('.') ? extension : '.' + extension);
                var unsafeName = WebUtility.HtmlEncode(product.ImageFile.FileName);
                
                // Create file in web root folder
                var path = Path.Combine(webHost.WebRootPath, "images", "carousel", safeName);
                await using FileStream fs = new(path, FileMode.Create);
                await product.ImageFile.CopyToAsync(fs);

                // Map product to the database entity
                var productToBeUpdated = new ProductEntity
                {
                    Name = product.ProductName,
                    CarouselItem = new CarouselItemEntity
                    {
                        Alt = product.ImageAlt,
                        Caption = product.ImageCaption,
                        Image = new ImageEntity
                        {
                            SafeName = safeName,
                            UnsafeName = unsafeName,
                        }
                    },
                    Price = product.ProductPrice
                };
                
                // Push modifications to the database
                sqlContext.Products.Add(productToBeUpdated);
                await sqlContext.SaveChangesAsync();
                
                return Ok(new { ProductIdentifier = productToBeUpdated.Id });
            }

            return UnprocessableEntity($"Image content type does not match any allowed mime types, allowed mime types: {string.Join(", ", FileExtensionHelper.Extensions.Keys)}");
        }
        catch (Exception e)
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            logger.Error(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { ErrorMassage = "Product update went wrong ..." });
        }
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] SqlContext sqlContext,
        [FromServices] ILogger logger,
        [FromForm] ProductModelForUpdateDto product)
    {
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(product);
        var validationResults = new List<ValidationResult>();
        if (Validator.TryValidateObject(product, context, validationResults, true) is false)
        {
            return UnprocessableEntity(new { ErrorMessage = "Product object validation encountered problems, errors: " + string.Join(", ", validationResults) });
        }

        if (product.ImageFile?.Length is >= Constants.MaximalImageSize or <= Constants.MinimalImageSize)
        {
            return UnprocessableEntity(new
            {
                ErrorMessage =
                    $"Invalid image file length, minimal file length is {Constants.MinimalImageSize} B and maximal {Constants.MaximalImageSize}"
            });
        }

        var productEntity = await (from p in sqlContext.Products
            join ci in sqlContext.CarouselItems on p.CarouselItemId equals ci.Id
            join i in sqlContext.Images on ci.Id equals i.CarouselItemId
            where p.Id == id
            select new
            {
                ProductIdentifier = p.Id,
                CarouselItemIdentifier = ci.Id,
                ImageIdentifier = i.Id,
                FileSafeName = i.SafeName,
                FileUnsageName = i.UnsafeName,
            }).SingleOrDefaultAsync();

        // Product to be updated must exist in the database
        if (productEntity is null)
        {
            return NotFound(new { ErrorMessage = $"Product with an ID: {id} does not exists within the database" });
        }

        // Map product to the database entity
        var productToBeUpdated = new ProductEntity
        {
            Id = productEntity.ProductIdentifier,
            Name = product.ProductName,
            CarouselItem = new CarouselItemEntity
            {
                Id = productEntity.CarouselItemIdentifier,
                Alt = product.ImageAlt,
                Caption = product.ImageCaption,
                Image = new ImageEntity
                {
                    Id = productEntity.ImageIdentifier,
                    SafeName = productEntity.FileSafeName,
                    UnsafeName = productEntity.FileUnsageName,
                    CarouselItemId = productEntity.CarouselItemIdentifier
                }
            },
            CarouselItemId = productEntity.CarouselItemIdentifier,
            Price = product.ProductPrice
        };

        try
        {
            // Check if image file has been included
            if (product.ImageFile is not null)
            {
                // Try get file extensions from mime types
                if (FileExtensionHelper.TryGetFileExtension(product.ImageFile.ContentType, out var extension))
                {
                    // Generate file names
                    var safeName = Path.GetRandomFileName() + (extension.Contains('.') ? extension : '.' + extension);
                    var unsafeName = WebUtility.HtmlEncode(product.ImageFile.FileName);
                    
                    // Create file in web root folder
                    var path = Path.Combine(webHost.WebRootPath, "images", "carousel", safeName);
                    await using FileStream fs = new(path, FileMode.Create);
                    await product.ImageFile.CopyToAsync(fs);

                    // Update file names in database
                    productToBeUpdated.CarouselItem.Image.SafeName = safeName;
                    productToBeUpdated.CarouselItem.Image.UnsafeName = unsafeName;
                    
                    // Delete old file in web root folder
                    path = Path.Combine(webHost.WebRootPath, "images", "carousel",
                        productEntity.FileSafeName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                else
                {
                    return UnprocessableEntity($"Image content type does not match any allowed mime types, allowed mime types: {string.Join(", ", FileExtensionHelper.Extensions.Keys)}");
                }
            }

            // Push modifications to the database
            sqlContext.Products.Update(productToBeUpdated);
            await sqlContext.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception e)
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            logger.Error(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { ErrorMassage = "Product update went wrong ..." });
        }
    }

    [HttpDelete("{id:int}", Name = nameof(DeleteAsync))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] IWebHostEnvironment webHost,
        [FromServices] ILogger logger,
        [FromServices] SqlContext sqlContext,
        int id)
    {
        var productModel = await (from p in sqlContext.Products
            join ci in sqlContext.CarouselItems on p.CarouselItemId equals ci.Id
            join i in sqlContext.Images on ci.Id equals i.CarouselItemId
            where p.Id == id
            select new
            {
                ProductIdentifier = p.Id,
                CarouselItemIdentifier = ci.Id,
                ImageIdentifier = i.Id,
                ImageSafeName = i.SafeName
            }).SingleOrDefaultAsync();

        if (productModel is null)
        {
            var msg = $"Product object with id: {id} does not exist in the database";
            logger.Information(msg);
            
            return NotFound(new { ErrorMessage = msg });
        }

        await using var sc = new SqlConnection(sqlContext.Database.GetConnectionString());
        await using var cmd = sc.CreateCommand();
        await sc.OpenAsync();

        cmd.CommandText = "DELETE FROM PRODUCTS WHERE ID == @PRODUCT_ID; DELETE FROM CAROUSELITEMS WHERE ID == @CAROUSEL_ITEM_ID; DELETE FROM IMAGES WHERE ID == @IMAGE_ID";
        cmd.Parameters.AddWithValue("@PRODUCT_ID", productModel.ProductIdentifier);
        cmd.Parameters.AddWithValue("@CAROUSEL_ITEM_ID", productModel.CarouselItemIdentifier);
        cmd.Parameters.AddWithValue("@IMAGE_ID", productModel.ImageIdentifier);
        var rowAffected = cmd.ExecuteNonQuery();
        //await sc.CloseAsync();

        if (rowAffected is 0)
        {
            logger.Error("Failed to delete resources from database. Resources: Product with ID: {ProductID}, Carousel Item with ID {CarouselItemID}, Image with ID: {ImageID}",
                productModel.ProductIdentifier,
                productModel.CarouselItemIdentifier,
                productModel.ImageIdentifier);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = $"Failed to delete resources from database. Resources: Product with ID: {productModel.ProductIdentifier}, Carousel Item with ID {productModel.CarouselItemIdentifier}, Image with ID: {productModel.ImageIdentifier}"});
        }

        if (string.IsNullOrEmpty(productModel.ImageSafeName))
        {
            return NoContent();
        }

        if (ImageFileHandler.TryDeleteImage(
                Path.Combine(webHost.WebRootPath, "images", "carousel", productModel.ImageSafeName),
                out var exceptionMessage))
        {
            return NoContent();
        }

        logger.Error(exceptionMessage);
        return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = exceptionMessage });
    }
}