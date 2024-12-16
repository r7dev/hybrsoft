using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Infrastructure.Enums;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public class AppLogDto : ObservableObject
	{
		static public AppLogDto CreateEmpty() => new() { AppLogId = -1, IsEmpty = true };

		public long AppLogId { get; set; }
		public bool IsRead { get; set; }
		public DateTimeOffset CreatedOn { get; set; }
		public string User { get; set; }
		public LogType Type { get; set; }
		public string Source { get; set; }
		public string Action { get; set; }
		public string Message { get; set; }
		public string Description { get; set; }
		public AppType AppType { get; set; }
	}
}
