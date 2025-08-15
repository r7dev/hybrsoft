using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
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
		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(AppLogListViewModel), "_Prefix"));

		public AppLogListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(AppLogListArgs args)
		{
			ViewModelArgs = args ?? AppLogListArgs.CreateEmpty();
			StartDate = ViewModelArgs.StartDate;
			EndDate = ViewModelArgs.EndDate;
			Query = ViewModelArgs.Query;

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogListViewModel), "_LoadingLogs"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogListViewModel), "_LogsLoaded"));
				EndStatusMessage(endMessage);
			}
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
				StatusError($"Error loading Logs: {ex.Message}");
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogListViewModel), "_LoadingLogs"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogListViewModel), "_LogsLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(AppLogListViewModel), "_AreYouSureYouWantToDeleteSelectedLogs"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(AppLogListViewModel), "_Deleting0Logs");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string resourceKey = string.Concat(nameof(AppLogListViewModel), "_ErrorDeleting0Logs1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("AppLogs", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(AppLogListViewModel), "_0LogsDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage(message, LogType.Warning);
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
