using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public partial class SQLServerDbLearn(string connectionString) : SQLServerDbBase(connectionString, schema), ILearnDataSource
	{
		private const string schema = "Learn";

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Student>().ToTable(nameof(Student), schema);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Student> Students { get; set; }
	}
}
