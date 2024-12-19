using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class AppLogsViewModel(ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public AppLogListViewModel AppLogList { get; } = new AppLogListViewModel(commonServices);
		public AppLogDetailsViewModel AppLogDetails { get; } = new AppLogDetailsViewModel(commonServices);

		public async Task LoadAsync(AppLogListArgs args)
		{
			await AppLogList.LoadAsync(args);
		}
		public void Unload()
		{
			AppLogList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
			AppLogList.Subscribe();
			AppLogDetails.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			AppLogList.Unsubscribe();
			AppLogDetails.Unsubscribe();
		}

		private async void OnMessage(AppLogListViewModel viewModel, string message, object args)
		{
			if (viewModel == AppLogList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (AppLogDetails.IsEditMode)
			{
				StatusReady();
			}
			var selected = AppLogList.SelectedItem;
			if (!AppLogList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
				}
			}
			AppLogDetails.Item = selected;
		}

		private async Task PopulateDetails(AppLogDto selected)
		{
			try
			{
				var model = await LogService.GetLogAsync(selected.AppLogId);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("AppLogs", "Load Details", ex);
			}
		}
	}
}
