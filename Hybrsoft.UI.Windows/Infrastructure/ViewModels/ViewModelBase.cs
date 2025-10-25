using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Diagnostics;

namespace Hybrsoft.UI.Windows.Infrastructure.ViewModels
{
	public partial class ViewModelBase(ICommonServices commonServices) : ObservableObject
	{
		private readonly Stopwatch _stopwatch = new();
		private readonly string _statusReadyMessage = commonServices.ResourceService.GetString<ViewModelBase>(ResourceFiles.InfoMessages, "Ready");

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

		public void StartStatusMessage(string title, string message)
		{
			StatusMessage(title, message);
			_stopwatch.Reset();
			_stopwatch.Start();
		}
		public void EndStatusMessage(string title, string message, LogType logType = LogType.Information)
		{
			_stopwatch.Stop();
			string finalMessage = ResourceService.GetString<ViewModelBase>(ResourceFiles.InfoMessages, "Seconds");
			string fullMessage = $"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} {finalMessage})";
			switch (logType)
			{
				case LogType.Success:
					SuccessMessage(title, fullMessage);
					break;
				case LogType.Warning:
					WarningMessage(title, fullMessage);
					break;
				default:
					StatusMessage(title, fullMessage);
					break;
			}
		}

		public void StatusReady()
		{
			string message = ResourceService.GetString<ViewModelBase>(ResourceFiles.InfoMessages, "Ready");
			MessageService.Send(this, "StatusMessage", new StatusInfoDto("", message));
		}

		public void StatusMessage(string title, string message)
		{
			MessageService.Send(this, "StatusMessage", new StatusInfoDto(title, message));
		}
		public void StatusError(string title, string message)
		{
			MessageService.Send(this, "StatusError", new StatusInfoDto(title, message));
		}
		public void SuccessMessage(string title, string message)
		{
			MessageService.Send(this, "SuccessMessage", new StatusInfoDto(title, message));
		}
		public void WarningMessage(string title, string message)
		{
			MessageService.Send(this, "WarningMessage", new StatusInfoDto(title, message));
		}

		public void StatusMessageYourself(string title, string message)
		{
			MessageService.SendYourself(this, "StatusMessage", new StatusInfoDto(title, message));
		}
		public void StatusErrorYourself(string title, string message)
		{
			MessageService.SendYourself(this, "StatusError", new StatusInfoDto(title, message));
		}
		public void SuccessMessageYourselt(string title, string message)
		{
			MessageService.SendYourself(this, "SuccessMessage", new StatusInfoDto(title, message));
		}
		public void WarningMessageYourself(string title, string message)
		{
			MessageService.SendYourself(this, "WarningMessage", new StatusInfoDto(title, message));
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
