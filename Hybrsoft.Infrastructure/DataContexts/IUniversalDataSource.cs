﻿using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public interface IUniversalDataSource : IDisposable
	{
		public DbSet<AppLog> AppLogs { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<CompanyUser> CompanyUsers { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<NavigationItem> NavigationItems { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<Subscription> Subscriptions { get; set; }
		public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

		EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

		int SaveChanges();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
