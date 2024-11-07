using System;

namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class ShellArgs
	{
		public Type ViewModel { get; set; }
		public object Parameter { get; set; }
		public UserInfo UserInfo { get; set; }
	}
}
