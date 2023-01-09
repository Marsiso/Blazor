namespace Blazor.Shared.Entities.DataTransferObjects;

public class ProcessedProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public double ProductPrice { get; set; }

    public string ImageAlt { get; set; }

    public string ImageCaption { get; set; }

    public string ImageSrc { get; set; }

    public int ProductQuantity { get; set; }

    public ProcessedProduct(int productId, string productName, double productPrice, string imageAlt,
        string imageCaption, string imageSrc, int productQuantity)
    {
        this.ProductId = productId;
        this.ProductName = productName;
        this.ProductPrice = productPrice;
        this.ImageAlt = imageAlt;
        this.ImageCaption = imageCaption;
        this.ImageSrc = imageSrc;
        this.ProductQuantity = productQuantity;
    }
}