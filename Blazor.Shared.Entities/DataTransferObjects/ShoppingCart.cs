namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ShoppingCart
{
    /// <summary>
    /// Key: Product unique identifier. Value: Product entity.
    /// </summary>
    public Dictionary<int, ProcessedProduct> Items { get; set; }

    public ShoppingCart()
    {
        Items = new Dictionary<int, ProcessedProduct>();
    }

    /// <summary>
    /// Adds new item to the shopping basket. If item already exists updates increments current quantity.
    /// </summary>
    /// <param name="product">Product to be added.</param>
    /// <param name="quantity">Product's quantity.</param>
    public void AddCartItem(ProcessedProduct product, int quantity)
    {
        if (Items.ContainsKey(product.ProductId))
        {
            Items[product.ProductId].ProductQuantity++;
        }
        else
        {
            Items.Add(product.ProductId, product);
            Items[product.ProductId].ProductQuantity++;
        }
    }

    public void RemoveCartItem(ProcessedProduct product)
    {
        if (!Items.ContainsKey(product.ProductId)) return;
        Items.Remove(product.ProductId);
    }

    public double TotalCost() => Items.Values.Sum(item => item.ProductQuantity * item.ProductPrice);
}