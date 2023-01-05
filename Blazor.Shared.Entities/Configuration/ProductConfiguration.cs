using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public sealed class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.HasData(new List<ProductEntity>
        {
            new()
            {
                Id = 1,
                CarouselItemId = 1,
                Name = "iPhone 14 Pro 128GB fialová",
                Price = 33_490
            },
            new()
            {
                Id = 2,
                CarouselItemId = 2,
                Name = "Garmin Tactix 7 Pro Solar Sapphire Ballistics",
                Price = 35_790
            },
            new()
            {
                Id = 3,
                CarouselItemId = 3,
                Name = "JURA E6 Platin",
                Price = 20_990
            },
            new()
            {
                Id = 4,
                CarouselItemId = 4,
                Name = "MSI GeForce RTX 3080 VENTUS 3X PLUS 10G OC LHR",
                Price = 22_990
            },
            new()
            {
                Id = 5,
                CarouselItemId = 5,
                Name = "AMD Ryzen 5 5600X",
                Price = 4_490
            }
        });
    }
}