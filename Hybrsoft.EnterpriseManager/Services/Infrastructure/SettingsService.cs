using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class SettingsService(IDialogService dialogService) : ISettingsService
	{
		public IDialogService DialogService { get; } = dialogService;

		public string UserName
		{
			get => AppSettings.Current.UserName;
			set => AppSettings.Current.UserName = value;
		}
	}
}
