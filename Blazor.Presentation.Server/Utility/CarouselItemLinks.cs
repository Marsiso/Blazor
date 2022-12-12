using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DataTransferObjects;
using Blazor.Shared.Entities.LinkModels;
using Blazor.Shared.Entities.Models;
using Microsoft.Net.Http.Headers;

namespace Blazor.Presentation.Server.Utility;

public sealed class CarouselItemLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<CarouselItemDto> _dataShaper;

    public CarouselItemLinks(LinkGenerator linkGenerator, IDataShaper<CarouselItemDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }
    
    public LinkResponse TryGenerateLinks(IEnumerable<CarouselItemDto> carouselItemsDto, string fields, HttpContext httpContext)
    {
        var shapedCarouselItems = ShapeData(carouselItemsDto, fields);
        if (ShouldGenerateLinks(httpContext))
        {
            return ReturnLinkedCarouselItems(carouselItemsDto, fields, httpContext, shapedCarouselItems);
        }

        return ReturnShapedCarouselItems(shapedCarouselItems);
    }
    
    private List<Entity> ShapeData(IEnumerable<CarouselItemDto> carouselItemsDto, string fields) =>
        _dataShaper.ShapeData(carouselItemsDto, fields)
            .Select(ci => ci.Entity)
            .ToList();
    
    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase); 
    } 
    
    private LinkResponse ReturnShapedCarouselItems(List<Entity> shapedCarouselItems) => new LinkResponse { ShapedEntities = shapedCarouselItems };
    
    private LinkResponse ReturnLinkedCarouselItems(IEnumerable<CarouselItemDto> carouselItemsDto, string fields,
        HttpContext httpContext, List<Entity> shapedCarouselItems)
    {
        var carouselItemDtoList = carouselItemsDto.ToList();
        for (var index = 0; index < carouselItemDtoList.Count; index++)
        {
            var carouselItemLinks = CreateLinksForCarouselItem(httpContext, carouselItemDtoList[index].Id, fields);
            _ = shapedCarouselItems[index].TryAdd("Links", carouselItemLinks);
        }

        var carouselItemsCollection = new LinkCollectionWrapper<Entity>(shapedCarouselItems);
        var linkedCarouselItems = CreateLinksForCarouselItem(httpContext, carouselItemsCollection);
        return new LinkResponse { HasLinks = true, LinkedEntities = linkedCarouselItems };
    }
    
    
    private List<Link> CreateLinksForCarouselItem(HttpContext httpContext, int carouselItemId, string fields = "")
    {
        var links = new List<Link>
        {
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "GetCarouselItem", "CarouselItem",
                    values: new { carouselItemId }), "self", "GET"),
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "DeleteCarouselItem", "CarouselItem", values: new { carouselItemId }),
                "delete_carousel_item", "DELETE"),
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "UpdateCarouselItem", "CarouselItem", values: new { carouselItemId }),
                "update_carousel_item", "PUT"),
            new Link(
                _linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateCarouselItem", "CarouselItem",
                    values: new { carouselItemId }), "partially_update_carousel_item", "PATCH")
        };
        
        return links;
    }
    
    private LinkCollectionWrapper<Entity> CreateLinksForCarouselItem(HttpContext httpContext,
        LinkCollectionWrapper<Entity> carouselItemsWrapper)
    {
        carouselItemsWrapper.Links.Add(new Link(
            _linkGenerator.GetUriByAction(httpContext, "GetAllCarouselItems", "CarouselItem", values: new { }), "self", "GET"));
        return carouselItemsWrapper;
    }
}