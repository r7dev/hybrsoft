using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class ShellViewModel(ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get => _isEnabled;
			set => Set(ref _isEnabled, value);
		}

		private string _message = "Ready";
		public string Message
		{
			get => _message;
			set => Set(ref _message, value);
		}

		private bool _isOpen;
		public bool IsOpen
		{
			get => _isOpen;
			set => Set(ref _isOpen, value);
		}

		private InfoBarSeverity _Severity;
		public InfoBarSeverity Severity
		{
			get => _Severity;
			set => Set(ref _Severity, value);
		}

		public UserInfo UserInfo { get; protected set; }

		public ShellArgs ViewModelArgs { get; protected set; }

		virtual public Task LoadAsync(ShellArgs args)
		{
			ViewModelArgs = args;
			if (ViewModelArgs != null)
			{
				UserInfo = ViewModelArgs.UserInfo;
				NavigationService.Navigate(ViewModelArgs.ViewModel, ViewModelArgs.Parameter);
			}
			return Task.CompletedTask;
		}

		virtual public void Unload()
		{
		}

		virtual public void Subscribe()
		{
			MessageService.Subscribe<ViewModelBase, string>(this, OnMessage);
		}

		virtual public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		private async void OnMessage(ViewModelBase viewModel, string message, string status)
		{
			switch (message)
			{
				case "SuccessMessage":
					if (viewModel.ContextService.ContextID == ContextService.ContextID)
					{
						IsOpen = !string.IsNullOrWhiteSpace(message);
						Severity = message == "SuccessMessage" ? InfoBarSeverity.Success : InfoBarSeverity.Informational;
						SetStatus(status);
					}
					break;
				case "StatusMessage":
				case "StatusError":
					if (viewModel.ContextService.ContextID == ContextService.ContextID)
					{
						IsOpen = !string.IsNullOrWhiteSpace(message);
						Severity = message == "StatusError" ? InfoBarSeverity.Error : InfoBarSeverity.Informational;
						SetStatus(status);
					}
					break;
				case "WarningMessage":
					if (viewModel.ContextService.ContextID == ContextService.ContextID)
					{
						IsOpen = !string.IsNullOrWhiteSpace(message);
						Severity = message == "WarningMessage" ? InfoBarSeverity.Warning : InfoBarSeverity.Informational;
						SetStatus(status);
					}
					break;

				case "EnableThisView":
				case "DisableThisView":
					if (viewModel.ContextService.ContextID == ContextService.ContextID)
					{
						IsEnabled = message == "EnableThisView";
						SetStatus(status);
					}
					break;

				case "EnableOtherViews":
				case "DisableOtherViews":
					if (viewModel.ContextService.ContextID != ContextService.ContextID)
					{
						await ContextService.RunAsync(() =>
						{
							IsEnabled = message == "EnableOtherViews";
							SetStatus(status);
						});
					}
					break;

				case "EnableAllViews":
				case "DisableAllViews":
					await ContextService.RunAsync(() =>
					{
						IsEnabled = message == "EnableAllViews";
						SetStatus(status);
					});
					break;
			}
		}

		private void SetStatus(string message)
		{
			message ??= "";
			message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
			Message = message;
		}
	}
}
