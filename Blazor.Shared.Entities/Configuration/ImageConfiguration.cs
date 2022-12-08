using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public class ImageConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.HasData(new List<ImageEntity>
        {
            new()
            {
                Id = 1,
                CarouselItemId = 1,
                SafeName = "adhdwe.jpg",
                UnsafeName = "image-01",
            },
            new()
            {
                Id = 2,
                CarouselItemId = 2,
                SafeName = "cxjuad.jpg",
                UnsafeName = "image-02",
            },
            new()
            {
                Id = 3,
                CarouselItemId = 3,
                SafeName = "ioyweq.jpg",
                UnsafeName = "image-03",
            },
            new()
            {
                Id = 4,
                CarouselItemId = 4,
                SafeName = "khfhew.jpg",
                UnsafeName = "image-04",
            },
        });
    }
}
