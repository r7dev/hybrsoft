using System;

namespace Hybrsoft.UI.Windows.Infrastructure.Services
{
	public interface ITitleService
	{
		string Title { get; set; }
		event EventHandler<string> TitleChanged;
	}
}
