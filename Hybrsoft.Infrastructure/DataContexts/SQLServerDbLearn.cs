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
			modelBuilder.Entity<Classroom>().ToTable(nameof(Classroom), schema);
			modelBuilder.Entity<ClassroomStudent>().ToTable(nameof(ClassroomStudent), schema);
			modelBuilder.Entity<ScheduleType>().ToTable(nameof(ScheduleType), schema);
			modelBuilder.Entity<Relative>().ToTable(nameof(Relative), schema);
			modelBuilder.Entity<RelativeType>().ToTable(nameof(RelativeType), schema);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Student> Students { get; set; }
		public DbSet<Classroom> Classrooms { get; set; }
		public DbSet<ClassroomStudent> ClassroomStudents { get; set; }
		public DbSet<ScheduleType> ScheduleTypes { get; set; }
		public DbSet<Relative> Relatives { get; set; }
		public DbSet<RelativeType> RelativeTypes { get; set; }
	}
}
