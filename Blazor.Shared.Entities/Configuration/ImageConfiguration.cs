﻿using Blazor.Shared.Entities.Models;
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
                SafeName = "RI043b4.jpg",
                UnsafeName = "img-iphone-14-pro-128gb-purple",
            },
            new()
            {
                Id = 2,
                CarouselItemId = 2,
                SafeName = "ImgW.jpg",
                UnsafeName = "img-garmin-tactix-7-pro-solar-sapphire-ballistics",
            },
            new()
            {
                Id = 3,
                CarouselItemId = 3,
                SafeName = "IAsDgW.jpg",
                UnsafeName = "img-jura-e6-platinum",
            },
            new()
            {
                Id = 4,
                CarouselItemId = 4,
                SafeName = "asdfgsd.jpg",
                UnsafeName = "img-msi-geforce-rtx-3080-ventus",
            },
            new()
            {
                Id = 5,
                CarouselItemId = 5,
                SafeName = "dfSDsd.jpg",
                UnsafeName = "img-amd-ryzen-5-5600x",
            }
        });
    }
}
