namespace Blazor.Shared.Entities.DataTransferObjects;

public record ProcessedProduct(int ProductId, string ProductName, double ProductPrice, string ImageAlt,
    string ImageCaption, string ImageSrc);