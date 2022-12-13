using Blazor.Shared.Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Presentation.Server.Controllers;

[ApiVersion("1.0")]
[Route("api")]
[ApiController]
public sealed class RootController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }
    
    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
    {
        if (mediaType.Contains("application/vnd.utb.apiroot"))
        {
            var list = new List<Link>
            {
                new Link()
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new { }), 
                    Rel = "self",
                    Method = "GET"
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, "GetAllCarouselItemsAsync", new { }), 
                    Rel = "carousel_items",
                    Method = "GET"
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, "CreateCarouselItemAsync", new { }), 
                    Rel = "create_carousel_item",
                    Method = "POST"
                }
            };
            
            return Ok(list);
        }

        return NoContent();
    }
}