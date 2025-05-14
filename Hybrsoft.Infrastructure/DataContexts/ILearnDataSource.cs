using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataContexts
{
	public interface ILearnDataSource : IDisposable
	{
		public DbSet<Student> Students { get; set; }
		public DbSet<Classroom> Classrooms { get; set; }
		public DbSet<ClassroomStudent> ClassroomStudents { get; set; }
		public DbSet<ScheduleType> ScheduleTypes { get; set; }

		EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

		int SaveChanges();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
