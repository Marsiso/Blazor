using AutoMapper;
using Blazor.Presentation.Server.Filters;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
    public async Task<IActionResult> GetImageAsync(int carouselItemId)
    {
        var imageEntity = HttpContext.Items["imageEntity"] as ImageEntity;

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
}
