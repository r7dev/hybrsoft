using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LostAndFoundListViewModel(ILostAndFoundService lostAndFoundService,
		ICommonServices commonServices) : GenericListViewModel<LostAndFoundModel>(commonServices)
	{
		private readonly ILostAndFoundService _lostAndFoundService = lostAndFoundService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.InfoMessages, "LoadingLostAndFound");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.InfoMessages, "LostAndFoundLoaded");
		public string Prefix => ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.UI, "Prefix");

		public LostAndFoundListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(LostAndFoundListArgs args)
		{
			ViewModelArgs = args ?? LostAndFoundListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			if (SelectedDataLayout == DataLayoutType.List)
				SelectedDataLayout = DataLayoutType.Grid;

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
			MessageService.Subscribe<LostAndFoundListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public LostAndFoundListArgs CreateArgs()
		{
			return new LostAndFoundListArgs
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
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
				string message = ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.Errors, "ErrorLoadingLostAndFound0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("LostAndFound", "Refresh", ex);
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

		private async Task<IList<LostAndFoundModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<LostAndFound> request = BuildDataRequest();
				return await _lostAndFoundService.GetLostAndFoundAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<LostAndFoundDetailsViewModel>(new LostAndFoundDetailsArgs { LostAndFoundID = SelectedItem.LostAndFoundID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<LostAndFoundDetailsViewModel>(new LostAndFoundDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<LostAndFoundDetailsViewModel>(new LostAndFoundDetailsArgs());
			}

			StatusReady();
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
			string content = ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedLostAndFound");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.InfoMessages, "Deleting0LostAndFound");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
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
					string message = ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.Errors, "ErrorDeleting0LostAndFound1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("LostAndFound", "Delete", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
						string message = ResourceService.GetString<LostAndFoundListViewModel>(ResourceFiles.InfoMessages, "0LostAndFoundDeleted");
						EndStatusMessage(title, string.Format(message, count), LogType.Warning);
					}
				}
				else
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString(ResourceFiles.Errors, "DeleteNotAllowed");
					StatusError(title, message);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<LostAndFoundModel> models)
		{
			foreach (var model in models)
			{
				await _lostAndFoundService.DeleteLostAndFoundAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<LostAndFound> request = BuildDataRequest();

			List<LostAndFoundModel> models = [];
			foreach (var range in ranges)
			{
				var items = await _lostAndFoundService.GetLostAndFoundAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await _lostAndFoundService.DeleteLostAndFoundRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<LostAndFound> BuildDataRequest()
		{
			return new DataRequest<LostAndFound>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
		}

		private void LogWarning(LostAndFoundModel model)
		{
			LogWarning("LostAndFound", "Delete", "LostAndFound deleted", $"LostAndFound {model.LostAndFoundID} '{model.DisplayName}' was deleted.");
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
	}
}
