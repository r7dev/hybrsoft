﻿using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public partial class SQLServerDbUniversal(string connectionString) : SQLServerDbBase(connectionString, schema), IUniversalDataSource
	{
		private const string schema = "Universal";

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AppLog>().ToTable(nameof(AppLog), schema);
			modelBuilder.Entity<Category>().ToTable(nameof(Category), schema);
			modelBuilder.Entity<NavigationItem>().ToTable(nameof(NavigationItem), schema);
			modelBuilder.Entity<Permission>().ToTable(nameof(Permission), schema);
			modelBuilder.Entity<Role>().ToTable(nameof(Role), schema);
			modelBuilder.Entity<RolePermission>().ToTable(nameof(RolePermission), schema);
			modelBuilder.Entity<User>().ToTable(nameof(User), schema);
			modelBuilder.Entity<UserRole>().ToTable(nameof(UserRole), schema);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<AppLog> AppLogs { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<NavigationItem> NavigationItems { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
	}
}
