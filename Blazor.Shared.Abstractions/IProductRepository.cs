using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Abstractions;

public interface IProductRepository
{
    Task<PagedList<ProductEntity>> GetAllProductsAsync(ProductParameters productParameters, bool trackChanges);
    Task<ProductEntity> GetProductAsync(int productId, bool trackChanges);
    void CreateProduct(ProductEntity product);
    void UpdateProduct(ProductEntity product);
    void DeleteProduct(ProductEntity product);
}