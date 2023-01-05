using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        builder.HasData(new List<OrderItemEntity>()
        {
            new()
            {
                Id = 1,
                Amount = 1,
                Price = 29_490,
                ProductId = 1,
                OrderId = 1
            },
            new()
            {
                Id = 2,
                Amount = 1,
                Price = 20_990,
                ProductId = 3,
                OrderId = 1
            },
            new()
            {
                Id = 3,
                Amount = 1,
                Price = 18_990,
                ProductId = 4,
                OrderId = 1
            },
            new()
            {
                Id = 4,
                Amount = 1,
                Price = 3_290,
                ProductId = 5,
                OrderId = 1
            },
            new()
            {
                Id = 5,
                Amount = 2,
                Price = 22_990,
                ProductId = 4,
                OrderId = 2
            },
            new()
            {
                Id = 6,
                Amount = 1,
                Price = 4_490,
                ProductId = 5,
                OrderId = 2
            },
            new()
            {
                Id = 7,
                Amount = 1,
                Price = 33_490,
                ProductId = 1,
                OrderId = 3
            },
            new()
            {
                Id = 8,
                Amount = 1,
                Price = 35_790,
                ProductId = 2,
                OrderId = 3
            }
        });
    }
}