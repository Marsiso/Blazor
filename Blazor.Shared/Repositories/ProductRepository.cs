using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Blazor.Shared.Implementations.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class ProductRepository : RepositoryBase<ProductEntity>, IProductRepository
{
    public ProductRepository(SqlContext context) : base(context)
    {
    }

    public async Task<PagedList<ProductEntity>> GetAllProductsAsync(ProductParameters productParameters, bool trackChanges)
    {
        var products = await FindAll(trackChanges)
            .FilterProducts(productParameters.MinPrice, productParameters.MaxPrice)
            .SearchProducts(productParameters.SearchTerm)
            .SortProducts(productParameters.OrderBy)
            .ToListAsync();
        
        return PagedList<ProductEntity>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);
    }

    public async Task<ProductEntity> GetProductAsync(int productId, bool trackChanges) =>
        await FindByCondition(product => product.Id == productId, trackChanges)
            .SingleOrDefaultAsync();

    public void CreateProduct(ProductEntity product) => Create(product);

    public void UpdateProduct(ProductEntity product) => Update(product);

    public void DeleteProduct(ProductEntity product) => Delete(product);
}