using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public class AppLogDb : DbContext
	{
		private string _connectionString = null;

		public AppLogDb(string connectionString)
		{
			_connectionString = connectionString;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connectionString);
		}

		public DbSet<AppLog> Logs { get; set; }

		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			appLog.DateTime = DateTime.UtcNow;
			Entry(appLog).State = EntityState.Added;
			return await SaveChangesAsync();
		}
	}
}
