using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class AppLogListViewModel(ICommonServices commonServices) : GenericListViewModel<AppLogModel>(commonServices)
	{
		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<AppLogListViewModel>(ResourceFiles.InfoMessages, "LoadingLogs");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<AppLogListViewModel>(ResourceFiles.InfoMessages, "LogsLoaded");
		public string Prefix => ResourceService.GetString<AppLogListViewModel>(ResourceFiles.UI, "Prefix");

		public AppLogListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(AppLogListArgs args)
		{
			ViewModelArgs = args ?? AppLogListArgs.CreateEmpty();
			StartDate = ViewModelArgs.StartDate;
			EndDate = ViewModelArgs.EndDate;
			Query = ViewModelArgs.Query;

			await RefreshWithStatusAsync();
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;

			// Release heavy collections.
			(Items as IDisposable)?.Dispose();
			Items = null;
			SelectedItems = null;
			SelectedIndexRanges = null;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
			MessageService.Subscribe<AppLogDetailsViewModel>(this, OnMessage);
			MessageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public AppLogListArgs CreateArgs()
		{
			return new AppLogListArgs
			{
				StartDate = StartDate ?? DateRangeTools.GetStartDate(),
				EndDate = EndDate ?? DateRangeTools.GetEndDate(),
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		public async Task<bool> RefreshAsync()
		{
			bool isOk = true;

			Items = null;
			ItemsCount = 0;
			SelectedItem = null;

			try
			{
				Items = await GetItemsAsync();
			}
			catch (Exception ex)
			{
				Items = [];
				string title = ResourceService.GetString(ResourceFiles.Errors, "LoadFailed");
				string message = ResourceService.GetString<AppLogListViewModel>(ResourceFiles.Errors, "ErrorLoadingLogs0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("AppLogs", "Refresh", ex);
				isOk = false;
			}

			ItemsCount = Items.Count;
			if (!IsMultipleSelection)
			{
				SelectedItem = Items.FirstOrDefault();
			}
			NotifyPropertyChanged(nameof(Title));

			return isOk;
		}

		private async Task<IList<AppLogModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<AppLog> request = BuildDataRequest();
				return await LogService.GetLogsAsync(request);
			}
			return [];
		}

		protected override void OnNew()
		{
			throw new NotImplementedException();
		}

		protected override async void OnRefresh()
		{
			await RefreshWithStatusAsync();
		}

		private async Task<bool> RefreshWithStatusAsync()
		{
			StartStatusMessage(StartTitle, StartMessage);
			bool isOk = await RefreshAsync();
			if (isOk)
			{
				EndStatusMessage(EndTitle, EndMessage);
			}
			return isOk;
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string dialogTitle = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<AppLogListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedLogs");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<AppLogListViewModel>(ResourceFiles.InfoMessages, "Deleting0Logs");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString<AppLogListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Logs1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("AppLogs", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<AppLogListViewModel>(ResourceFiles.InfoMessages, "0LogsDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<AppLogModel> models)
		{
			foreach (var model in models)
			{
				await LogService.DeleteLogAsync(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<AppLog> request = BuildDataRequest();
			foreach (var range in ranges.Reverse())
			{
				await LogService.DeleteLogRangeAsync(range.Index, range.Length, request);
			}
		}

		private DataRequest<AppLog> BuildDataRequest()
		{
			return new DataRequest<AppLog>()
			{
				Query = Query,
				Where = r => r.AppType == AppType.EnterpriseManager
					&& r.CreateOn >= StartDate
					&& r.CreateOn <= EndDate,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemDeleted":
				case "ItemsDeleted":
				case "ItemRangesDeleted":
					await ContextService.RunAsync(async () =>
					{
						await RefreshAsync();
					});
					break;
			}
		}

		private async void OnLogServiceMessage(ILogService logService, string message, AppLog log)
		{
			if (message == "LogAdded" && log.Action != "Closing")
			{
				await ContextService.RunAsync(async () =>
				{
					await RefreshAsync();
				});
			}
		}
	}
}
