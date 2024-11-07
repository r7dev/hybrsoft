using Hybrsoft.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hybrsoft.Infrastructure.Models
{
	public class AppLog
	{
		[Key]
		public long Id { get; set; }

		public bool IsRead { get; set; }

		public string Name { get; set; }

		[Required]
		public DateTimeOffset DateTime { get; set; }

		[Required]
		[MaxLength(50)]
		public string User { get; set; }

		[Required]
		public LogType Type { get; set; }

		[Required]
		[MaxLength(50)]
		public string Source { get; set; }

		[Required]
		[MaxLength(50)]
		public string Action { get; set; }

		[Required]
		[MaxLength(400)]
		public string Message { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }
	}
}
