using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public abstract class SQLServerDbBase(string connectionString, string schema) : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(connectionString);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(schema);
			base.OnModelCreating(modelBuilder);
		}
	}
}
