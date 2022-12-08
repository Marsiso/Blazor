using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Enums;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Text;

namespace Blazor.Presentation.Server.Controllers;

[Route("api/CarouselItem/{carouselItemId:int}/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    readonly Serilog.ILogger _logger;
    readonly IRepositoryManager _repository;
    readonly IMapper _mapper;
    readonly IWebHostEnvironment _webHost;

    public ImageController(Serilog.ILogger logger, IRepositoryManager repository, IMapper mapper, IWebHostEnvironment webHost)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _webHost = webHost;
    }


    [HttpGet(Name = "GetImageAsync")]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter), Order = 1)]
    [ServiceFilter(typeof(ImageExistsValidationFilter), Order = 2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetImageAsync(int carouselItemId)
    {
        var imageEntity = HttpContext.Items[nameof(ImageEntity)] as ImageEntity;

        var basePath = _webHost.WebRootPath;
        var path = Path.Combine(basePath, "images", "carousel", imageEntity.SafeName);
        if (!System.IO.File.Exists(path))
        {
            _logger.Information("Image with id: {Id} doesn't have file in local storage", imageEntity.Id);
            return NotFound();
        }

        var imageDto = _mapper.Map<ImageDto>(imageEntity);

        var bytes = await System.IO.File.ReadAllBytesAsync(path);
        var payload = Convert.ToBase64String(bytes);
        var result = new FileExtensionContentTypeProvider().TryGetContentType(imageEntity.SafeName, out var contentType);
        if (!result)
        {
            _logger.Information("Failed to convert extension to content type for file: {File}", imageEntity.SafeName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var strBuilder = new StringBuilder();
        strBuilder.Append("data:").Append(contentType).Append(";base64,").Append(payload);
        imageDto.Src = strBuilder.ToString();

        return Ok(imageDto);
    }

    [HttpPost(Name = "CreateImageAsync")]
    [ServiceFilter(typeof(ValidationFilter), Order = 1)]
    [ServiceFilter(typeof(CarouselItemExistsValidationFilter), Order = 2)]
    [ServiceFilter(typeof(ImageFormatValidationFilter), Order = 3)]
    [ServiceFilter(typeof(ImageSizeValidationFilter), Order = 4)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateImageAsync(int carouselItemId, [FromForm] ImageForCreationDto image)
    {
        var trustedNameForDisplay = WebUtility.HtmlEncode(image.FileName); ;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        var extension = HttpContext.Items["extension"] as string;

        try
        {
            // Build path
            string path, trustedNameForStorage;
            do
            {
                var rndFileName = Path.GetRandomFileName();
                trustedNameForStorage = Path.ChangeExtension(rndFileName, Path.GetExtension(rndFileName) + extension);
                path = Path.Combine(_webHost.WebRootPath, "images", "carousel", trustedNameForStorage);
            } while (System.IO.File.Exists(path));

            // Update database
            var carouselItemEntity = HttpContext.Items[nameof(CarouselItemEntity)] as CarouselItemEntity;
            var imageEntity = await _repository.Image.GetImageByCarouselItemAsync(carouselItemEntity, true);
            if (imageEntity != null)
            {
                var imgToDeletePath = Path.Combine(_webHost.WebRootPath, "images", "carousel", imageEntity.SafeName);
                if (System.IO.File.Exists(imgToDeletePath))
                {
                    System.IO.File.Delete(imgToDeletePath);
                }

                imageEntity.UnsafeName = trustedNameForDisplay;
                imageEntity.SafeName = trustedNameForStorage;
                _repository.Image.UpdateImage(imageEntity);
                _logger.Information("{EntityName} associated with carousel item with id: {ID} has been updated",
                    nameof(ImageEntity),
                    carouselItemId);
            }
            else
            {
                var imageEntityToCreate = new ImageEntity
                {
                    UnsafeName = trustedNameForDisplay,
                    SafeName = trustedNameForStorage,
                    CarouselItemId = carouselItemId
                };

                _repository.Image.CreateImage(imageEntityToCreate);
                _logger.Information("{EntityName} associated with carousel item with id: {ID} has been created",
                    nameof(ImageEntity),
                    carouselItemId);
            }

            await _repository.SaveAsync();

            // Create file at path
            await using FileStream fs = new(path, FileMode.Create);
            await image.ImageFile.CopyToAsync(fs);

            _logger.Information("Image {FileName} saved at {Path}",
                trustedNameForDisplay,
                path);
        }
        catch (Exception ex)
        {
            _logger.Error("{FileName} error on upload (Err: {ErrorCode}): {Message}",
                trustedNameForDisplay,
                (int)ErrorCodes.FileOnUploadError,
                ex.Message);

            return StatusCode(StatusCodes.Status500InternalServerError, String.Format("File {0} error on upload (Err: {1}): {2}", image.FileName, (int)ErrorCodes.FileOnUploadError, ex.Message));
        }

        return Created(resourcePath, new { });
    }
}
