using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public partial class SQLServerDb(string connectionString) : DbContext, IDataSource
	{
		private const string SchemaUniversal = "Universal";
		private readonly string _connectionString = connectionString;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connectionString);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(SchemaUniversal);
			modelBuilder.Entity<AppLog>().ToTable("AppLog", SchemaUniversal);
			modelBuilder.Entity<Category>().ToTable("Category", SchemaUniversal);
			modelBuilder.Entity<NavigationItem>().ToTable("NavigationItem", SchemaUniversal);
			modelBuilder.Entity<Permission>().ToTable("Permission", SchemaUniversal);
			modelBuilder.Entity<Role>().ToTable("Role", SchemaUniversal);
			modelBuilder.Entity<RolePermission>().ToTable("RolePermission", SchemaUniversal);
			modelBuilder.Entity<User>().ToTable("User", SchemaUniversal);
			modelBuilder.Entity<UserRole>().ToTable("UserRole", SchemaUniversal);
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
