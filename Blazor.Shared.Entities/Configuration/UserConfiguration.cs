using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasData(new List<UserEntity>
        {
            new()
            {
                Id = 1,
                Country = "France",
                Name = "Tomas Adamek",
                FirstName = "Tomas",
                LastName = "Adamek",
                Email = "t_adamek@utb.cz",
                Password = "Password9910014785",
                Address = "Paris",
                Roles = new[] { Identity.Roles.Administrator, Identity.Roles.Manager, Identity.Roles.Visitor }
            },
            new()
            {
                Id = 2,
                Country = "Czech Republic",
                Name = "Marek Olsak",
                FirstName = "Marek",
                LastName = "Olsak",
                Email = "m_olsak@outlook.cz",
                Password = "Password9910014785",
                Address = "Prague",
                Roles = new[] { Identity.Roles.Manager, Identity.Roles.Visitor }
            },
            new()
            {
                Id = 3,
                Country = "Czech Republic",
                Name = "Hanka Chrencikova",
                FirstName = "Hanka",
                LastName = "Chrencikova",
                Email = "h_chrencikova@utb.cz",
                Password = "Password9910014785",
                Address = "Halenkovice",
                Roles = new[] { Identity.Roles.Manager, Identity.Roles.Visitor }
            }
        });
    }
}