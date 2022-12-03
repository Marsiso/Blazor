using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public class CarouselItemConfiguration : IEntityTypeConfiguration<CarouselItemEntity>

{
    public void Configure(EntityTypeBuilder<CarouselItemEntity> builder)
    {
        builder.HasData(new List<CarouselItemEntity>
        {
            new()
            {
                Id = 1,
                Alt = "First image",
                Src = "images/carousel/image-01.jpg",
                Caption = "First image"
            },
            new()
            {
                Id = 2,
                Alt = "Second image",
                Src = "images/carousel/image-02.jpg",
                Caption = "Second image"
            },
            new()
            {
                Id = 3,
                Alt = "Third image",
                Src = "images/carousel/image-03.jpg",
                Caption = "Third image"
            },
            new()
            {
                Id = 4,
                Alt = "Fourth image",
                Src = "images/carousel/image-04.jpg",
                Caption = "Fourth image"
            }
        });
    }
}
