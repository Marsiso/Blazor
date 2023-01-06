using Blazor.Shared.Entities.Configuration;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Entities.DbContexts;

public class SqlContext : DbContext
{
	public DbSet<ImageEntity> Images { get; set; }
	public DbSet<CarouselItemEntity> CarouselItems { get; set; }
	public DbSet<ProductEntity> Products { get; set; }
	public DbSet<OrderItemEntity> OrderItems { get; set; }
	public DbSet<OrderEntity> Orders { get; set; }
	public DbSet<UserEntity> Users { get; set; }
	public DbSet<ResetPasswordRequestEntity> ResetPasswordRequests { get; set; }

	public SqlContext(DbContextOptions<SqlContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserEntity>()
			.Property(user => user.Roles)
			.HasConversion(
				arrayOfStrings => String.Join(',', arrayOfStrings),
				arrayOfStringsAsString => arrayOfStringsAsString.Split(',', StringSplitOptions.RemoveEmptyEntries));

		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new OrderConfiguration());
		modelBuilder.ApplyConfiguration(new ProductConfiguration());
		modelBuilder.ApplyConfiguration(new CarouselItemConfiguration());
		modelBuilder.ApplyConfiguration(new ImageConfiguration());
		modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
	}
}
