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
                SafeName = "img-iphone-14-pro-128gb-purple.jpg",
                UnsafeName = "img-iphone-14-pro-128gb-purple.jpg",
            },
            new()
            {
                Id = 2,
                CarouselItemId = 2,
                SafeName = "img-garmin-tactix-7-pro-solar-sapphire-ballistics.jpg",
                UnsafeName = "img-garmin-tactix-7-pro-solar-sapphire-ballistics.jpg",
            },
            new()
            {
                Id = 3,
                CarouselItemId = 3,
                SafeName = "img-jura-e6-platinum.jpg",
                UnsafeName = "img-jura-e6-platinum.jpg",
            },
            new()
            {
                Id = 4,
                CarouselItemId = 4,
                SafeName = "img-msi-geforce-rtx-3080-ventus.jpg",
                UnsafeName = "img-msi-geforce-rtx-3080-ventus.jpg",
            },
            new()
            {
                Id = 5,
                CarouselItemId = 5,
                SafeName = "img-amd-ryzen-5-5600x.jpg",
                UnsafeName = "img-amd-ryzen-5-5600x.jpg",
            }
        });
    }
}
