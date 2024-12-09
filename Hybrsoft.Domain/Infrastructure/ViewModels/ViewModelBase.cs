using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Hybrsoft.Domain.Infrastructure.ViewModels
{
	public class ViewModelBase : ObservableObject
	{
		private Stopwatch _stopwatch = new Stopwatch();

		public ViewModelBase(ICommonServices commonServices)
		{
			ContextService = commonServices.ContextService;
			NavigationService = commonServices.NavigationService;
			MessageService = commonServices.MessageService;
			DialogService = commonServices.DialogService;
			LogService = commonServices.LogService;
		}

		public IContextService ContextService { get; }
		public INavigationService NavigationService { get; }
		public IMessageService MessageService { get; }
		public IDialogService DialogService { get; }
		public ILogService LogService { get; }

		public bool IsMainView => ContextService.IsMainView;

		virtual public string Title => String.Empty;

		public async void LogInformation(string source, string action, string message, string description)
		{
			await LogService.WriteAsync(LogType.Information, source, action, message, description);
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
		public void EndStatusMessage(string message, InfoBarSeverity severity = InfoBarSeverity.Informational)
		{
			_stopwatch.Stop();
			string fullMessage = $"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} seconds)";
			switch (severity)
			{
				case InfoBarSeverity.Success:
					SucessMessage(fullMessage);
					break;
				default:
					StatusMessage(fullMessage);
					break;
			}
		}

		public void StatusReady()
		{
			MessageService.Send(this, "StatusMessage", "Ready");
		}

		public void StatusMessage(string message)
		{
			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
			MessageService.Send(this, "StatusMessage", message);
		}
		public void StatusError(string message)
		{
			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
			MessageService.Send(this, "StatusError", message);
		}
		public void SucessMessage(string message)
		{
			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
			MessageService.Send(this, "SucessMessage", message);
		}
	}
}
