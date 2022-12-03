using Blazor.Shared.Entities.Configuration;
using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Entities.DbContexts;

public class SqlContext : DbContext
{
	public DbSet<CarouselItemEntity> CarouselItems { get; set; }
	public DbSet<ImageEntity> Images { get; set; }

	public SqlContext(DbContextOptions<SqlContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new CarouselItemConfiguration());
		modelBuilder.ApplyConfiguration(new ImageConfiguration());
	}
}
