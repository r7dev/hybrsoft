using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Diagnostics;

namespace Hybrsoft.UI.Windows.Infrastructure.ViewModels
{
	public partial class ViewModelBase(ICommonServices commonServices) : ObservableObject
	{
		private readonly Stopwatch _stopwatch = new();
		private readonly string _statusReadyMessage = commonServices.ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(ViewModelBase)}_Ready");

		public IContextService ContextService { get; } = commonServices.ContextService;
		public INavigationService NavigationService { get; } = commonServices.NavigationService;
		public IMessageService MessageService { get; } = commonServices.MessageService;
		public IDialogService DialogService { get; } = commonServices.DialogService;
		public ILogService LogService { get; } = commonServices.LogService;
		public IAuthorizationService AuthorizationService { get; } = commonServices.AuthorizationService;
		public IResourceService ResourceService { get; } = commonServices.ResourceService;

		public bool IsMainView => ContextService.IsMainView;

		virtual public string Title => String.Empty;

		public async void LogInformation(string source, string action, string message, string description)
		{
			await LogService.WriteAsync(LogType.Information, source, action, message, description);
		}

		public async void LogSuccess(string source, string action, string message, string description)
		{
			await LogService.WriteAsync(LogType.Success, source, action, message, description);
		}

		public async void LogWarning(string source, string action, string message, string description)
		{
			await LogService.WriteAsync(LogType.Warning, source, action, message, description);
		}

		public void LogException(string source, string action, Exception exception)
		{
			LogError(source, action, exception.Message, exception.ToString());
		}
		public async void LogError(string source, string action, string message, string description)
		{
			await LogService.WriteAsync(LogType.Error, source, action, message, description);
		}

		public void StartStatusMessage(string message)
		{
			StatusMessage(message);
			_stopwatch.Reset();
			_stopwatch.Start();
		}
		public void EndStatusMessage(string message, LogType logType = LogType.Information)
		{
			_stopwatch.Stop();
			string finalMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(ViewModelBase)}_Seconds");
			string fullMessage = $"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} {finalMessage})";
			switch (logType)
			{
				case LogType.Success:
					SuccessMessage(fullMessage);
					break;
				case LogType.Warning:
					WarningMessage(fullMessage);
					break;
				default:
					StatusMessage(fullMessage);
					break;
			}
		}

		public void StatusReady()
		{
			string message = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(ViewModelBase)}_Ready");
			MessageService.Send(this, "StatusMessage", message);
		}

		public void StatusMessage(string message)
		{
			MessageService.Send(this, "StatusMessage", message);
		}
		public void StatusError(string message)
		{
			MessageService.Send(this, "StatusError", message);
		}
		public void SuccessMessage(string message)
		{
			MessageService.Send(this, "SuccessMessage", message);
		}
		public void WarningMessage(string message)
		{
			MessageService.Send(this, "WarningMessage", message);
		}

		public void StatusMessageYourself(string message)
		{
			MessageService.SendYourself(this, "StatusMessage", message);
		}
		public void StatusErrorYourself(string message)
		{
			MessageService.SendYourself(this, "StatusError", message);
		}
		public void SuccessMessageYourselt(string message)
		{
			MessageService.SendYourself(this, "SuccessMessage", message);
		}

		public void EnableThisView(string message = null)
		{
			message = message ?? _statusReadyMessage;
			MessageService.Send(this, "EnableThisView", message);
		}
		public void DisableThisView(string message)
		{
			MessageService.Send(this, "DisableThisView", message);
		}

		public void EnableOtherViews(string message = null)
		{
			message = message ?? _statusReadyMessage;
			MessageService.Send(this, "EnableOtherViews", message);
		}
		public void DisableOtherViews(string message)
		{
			MessageService.Send(this, "DisableOtherViews", message);
		}

		public void EnableAllViews(string message = null)
		{
			message = message ?? _statusReadyMessage;
			MessageService.Send(this, "EnableAllViews", message);
		}
		public void DisableAllViews(string message)
		{
			MessageService.Send(this, "DisableAllViews", message);
		}
	}
}
