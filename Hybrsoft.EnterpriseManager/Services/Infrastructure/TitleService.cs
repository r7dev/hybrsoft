using Hybrsoft.UI.Windows.Infrastructure.Services;
using System;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class TitleService : ITitleService
	{
		private string _title;
		public string Title
		{
			get => _title;
			set
			{
				if (_title != value)
				{
					_title = value;
					TitleChanged?.Invoke(this, _title);
				}
			}
		}

		public event EventHandler<string> TitleChanged;
	}
}
