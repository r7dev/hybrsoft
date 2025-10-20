using Hybrsoft.UI.Windows.Infrastructure.Common;
using System;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class ShellArgs
	{
		public Type ViewModel { get; set; }
		public object Parameter { get; set; }
		public UserInfo UserInfo { get; set; }
	}
}
