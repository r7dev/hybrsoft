using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public interface IDataSource : IDisposable
	{
		public DbSet<AppLog> Logs { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<NavigationItem> NavigationItems { get; set; }

		EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

		int SaveChanges();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
