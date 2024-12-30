using Hybrsoft.Domain.Infrastructure.Commom;
using System;

namespace Hybrsoft.Domain.ViewModels
{
	public class ShellArgs
	{
		public Type ViewModel { get; set; }
		public object Parameter { get; set; }
		public UserInfo UserInfo { get; set; }
	}
}
