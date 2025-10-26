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
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RelativeListViewModel(IRelativeService relativeService, ICommonServices commonServices) : GenericListViewModel<RelativeModel>(commonServices)
	{
		public IRelativeService RelativeService { get; } = relativeService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<RelativeListViewModel>(ResourceFiles.InfoMessages, "LoadingRelatives");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<RelativeListViewModel>(ResourceFiles.InfoMessages, "RelativesLoaded");
		public string Prefix => ResourceService.GetString<RelativeListViewModel>(ResourceFiles.UI, "Prefix");

		public RelativeListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RelativeListArgs args)
		{
			ViewModelArgs = args ?? RelativeListArgs.CreateEmpty();
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
			MessageService.Subscribe<RelativeListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public RelativeListArgs CreateArgs()
		{
			return new RelativeListArgs
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
				string message = ResourceService.GetString<RelativeListViewModel>(ResourceFiles.Errors, "ErrorLoadingRelatives0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Relatives", "Refresh", ex);
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

		private async Task<IList<RelativeModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Relative> request = BuildDataRequest();
				return await RelativeService.GetRelativesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<RelativeDetailsViewModel>(new RelativeDetailsArgs { RelativeID = SelectedItem.RelativeID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<RelativeDetailsViewModel>(new RelativeDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<RelativeDetailsViewModel>(new RelativeDetailsArgs());
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
			string content = ResourceService.GetString<RelativeListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedRelatives");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<RelativeListViewModel>(ResourceFiles.InfoMessages, "Deleting0Relatives");
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
					string message = ResourceService.GetString<RelativeListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Relatives1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("Relatives", "Delete", ex);
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
						string message = ResourceService.GetString<RelativeListViewModel>(ResourceFiles.InfoMessages, "0RelativesDeleted");
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

		private async Task DeleteItemsAsync(IEnumerable<RelativeModel> models)
		{
			foreach (var model in models)
			{
				await RelativeService.DeleteRelativeAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Relative> request = BuildDataRequest();

			List<RelativeModel> models = [];
			foreach (var range in ranges)
			{
				var items = await RelativeService.GetRelativesAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await RelativeService.DeleteRelativeRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Relative> BuildDataRequest()
		{
			return new DataRequest<Relative>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
		}

		private void LogWarning(RelativeModel model)
		{
			LogWarning("Relative", "Delete", "Relative deleted", $"Relative {model.RelativeID} '{model.FullName}' was deleted.");
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
