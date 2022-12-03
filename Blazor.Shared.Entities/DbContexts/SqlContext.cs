using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Entities.DbContexts;

public class SqlContext : DbContext
{
	public SqlContext(DbContextOptions<SqlContext> options) : base(options)
	{
	}

	public DbSet<CarouselItem> CarouselItems { get; set; }
	public DbSet<Image> Images { get; set; }
}
