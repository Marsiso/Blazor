using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;

namespace Blazor.Shared.Implementations.Repositories;

public class ImageRepository : RepositoryBase<ImageEntity>, IImageRepository
{
    public ImageRepository(SqlContext context) : base(context)
    {
    }
}
