using Hybrsoft.Enums;
using System;
using System.Collections.Generic;

namespace Hybrsoft.Infrastructure.Models
{
	public class AppLog
	{
		public long AppLogID { get; set; }
		public bool IsRead { get; set; }
		public DateTimeOffset CreateOn { get; set; }
		public string User { get; set; }
		public LogType Type { get; set; }
		public string Source { get; set; }
		public string Action { get; set; }
		public string Message { get; set; }
		public string Description { get; set; }
		public AppType AppType { get; set; }
		public string SearchTerms { get; set; }

		public ICollection<AppLogEmbedding> AppLogEmbeddings { get; set; }
	}
}
