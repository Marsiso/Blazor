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
                Caption = "First image"
            },
            new()
            {
                Id = 2,
                Alt = "Second image",
                Caption = "Second image"
            },
            new()
            {
                Id = 3,
                Alt = "Third image",
                Caption = "Third image"
            },
            new()
            {
                Id = 4,
                Alt = "Fourth image",
                Caption = "Fourth image"
            }
        });
    }
}
