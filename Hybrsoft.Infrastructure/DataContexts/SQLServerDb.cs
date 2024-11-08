﻿using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public class SQLServerDb : DbContext, IDataSource
	{
		private string _connectionString = null;

		public SQLServerDb(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connectionString);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("Universal");
			modelBuilder.Entity<User>().ToTable("User", "Universal");
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Menu> Menu { get; set; }
		public DbSet<AppLog> Logs { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
