﻿using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class AppLogListViewModel(ICommonServices commonServices) : GenericListViewModel<AppLogDto>(commonServices)
	{
		public AppLogListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(AppLogListArgs args)
		{
			ViewModelArgs = args ?? AppLogListArgs.CreateEmpty();
			StartDate = ViewModelArgs.StartDate;
			EndDate = ViewModelArgs.EndDate;
			Query = ViewModelArgs.Query;

			StartStatusMessage("Loading logs...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Logs loaded");
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
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

		private async Task<IList<AppLogDto>> GetItemsAsync()
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
			StartStatusMessage("Loading logs...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Logs loaded");
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected logs?", "Ok", "Cancel"))
			{
				int count = 0;
				try
				{
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage($"Deleting {count} logs...");
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage($"Deleting {count} logs...");
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					StatusError($"Error deleting {count} Logs: {ex.Message}");
					LogException("AppLogs", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					EndStatusMessage($"{count} logs deleted", LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<AppLogDto> models)
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
			if (message == "LogAdded")
			{
				await ContextService.RunAsync(async () =>
				{
					await RefreshAsync();
				});
			}
		}
	}
}
