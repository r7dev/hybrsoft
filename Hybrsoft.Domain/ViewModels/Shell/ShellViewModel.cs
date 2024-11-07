﻿using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
    public class ShellViewModel : ViewModelBase
	{
		public ShellViewModel(ICommonServices commonServices) : base(commonServices)
		{
			
		}

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

		private bool _isError = false;
		public bool IsError
		{
			get => _isError;
			set => Set(ref _isError, value);
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
				case "StatusMessage":
				case "StatusError":
					if (viewModel.ContextService.ContextID == ContextService.ContextID)
					{
						IsError = message == "StatusError";
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
			message = message ?? "";
			message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
			Message = message;
		}
	}
}
