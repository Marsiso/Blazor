using System.Globalization;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.HasData(new List<OrderEntity>()
        {
            new()
            {
                Id = 1,
                UserId = 1,
                DateTimeCreated = new DateTime(2021, 5, 8, 14, 40, 52, 313), // DateTime.ParseExact("2021-05-08 14:40:52,313", "yyyy-MM-dd HH:mm:ss,fff",CultureInfo.InvariantCulture)
                TotalPrice = 72_760,
                OrderNumber = Guid.NewGuid().ToString()
            },
            new()
            {
                Id = 2,
                UserId = 1,
                DateTimeCreated = new DateTime(2022, 3, 11, 21, 40, 1, 121), /*DateTime.ParseExact("2022-03-11 21:40:01,121", "yyyy-MM-dd HH:mm:ss,fff",
                    CultureInfo.InvariantCulture)*/
                TotalPrice = 49_040,
                OrderNumber = Guid.NewGuid().ToString()
            },
            new()
            {
                Id = 3,
                UserId = 1,
                DateTimeCreated = new DateTime(2022,12,25,11,30,42,531), // DateTime.ParseExact("2022-12-25 11:30:42,531", "yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture)
                TotalPrice = 69_280,
                OrderNumber = Guid.NewGuid().ToString()
            }
        });
    }
}