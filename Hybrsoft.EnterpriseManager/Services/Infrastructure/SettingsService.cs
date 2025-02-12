using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class SettingsService(IDialogService dialogService) : ISettingsService
	{
		public IDialogService DialogService { get; } = dialogService;

		public string AppName => AppSettings.Current.AppName;
		public string Version => AppSettings.Current.Version;

		public long UserID
		{
			get => AppSettings.Current.UserID;
			set => AppSettings.Current.UserID = value;
		}

		public string UserName
		{
			get => AppSettings.Current.UserName;
			set => AppSettings.Current.UserName = value;
		}

		public char PasswordChar
		{
			get => AppSettings.Current.PasswordChar;
		}
	}
}
