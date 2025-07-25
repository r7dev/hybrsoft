﻿namespace Hybrsoft.Domain.ViewModels
{
	public class RelativeDetailsArgs
	{
		public static RelativeDetailsArgs CreateDefault() => new();

		public long RelativeID { get; set; }

		public bool IsNew => RelativeID <= 0;
	}
}
