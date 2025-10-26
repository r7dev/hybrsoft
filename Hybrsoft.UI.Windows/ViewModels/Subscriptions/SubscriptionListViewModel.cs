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
	public partial class SubscriptionListViewModel(ISubscriptionService subscriptionService, ICommonServices commonServices) : GenericListViewModel<SubscriptionModel>(commonServices)
	{
		public ISubscriptionService SubscriptionService { get; } = subscriptionService;

		private bool _hasEditorPermission;
		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.InfoMessages, "LoadingSubscriptions");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.InfoMessages, "SubscriptionsLoaded");
		public string Prefix => ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.UI, "Prefix");

		public SubscriptionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(SubscriptionListArgs args)
		{
			ViewModelArgs = args ?? SubscriptionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasEditorPermission = AuthorizationService.HasPermission(Permissions.SubscriptionEditor);

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
			MessageService.Subscribe<SubscriptionListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public SubscriptionListArgs CreateArgs()
		{
			return new SubscriptionListArgs
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
				string message = ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.Errors, "ErrorLoadingSubscriptions0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Subscriptions", "Refresh", ex);
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

		private async Task<IList<SubscriptionModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Subscription> request = BuildDataRequest();
				return await SubscriptionService.GetSubscriptionsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<SubscriptionDetailsViewModel>(new SubscriptionDetailsArgs { SubscriptionID = SelectedItem.SubscriptionID });
			}
		}

		public new ICommand NewCommand => new RelayCommand(OnNew, CanNew);
		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<SubscriptionDetailsViewModel>(new SubscriptionDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<SubscriptionDetailsViewModel>(new SubscriptionDetailsArgs());
			}

			StatusReady();
		}
		private bool CanNew()
		{
			return _hasEditorPermission;
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

		public new ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection, CanDeleteSelection);
		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string dialogTitle = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedSubscriptions");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.InfoMessages, "Deleting0Subscriptions");
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
					string message = ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Subscriptions1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("Subscriptions", "Delete", ex);
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
						string resourceValue = ResourceService.GetString<SubscriptionListViewModel>(ResourceFiles.InfoMessages, "0SubscriptionsDeleted");
						string message = string.Format(resourceValue, count);
						EndStatusMessage(title, message, LogType.Warning);
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
		private bool CanDeleteSelection()
		{
			return _hasEditorPermission;
		}

		private async Task DeleteItemsAsync(IEnumerable<SubscriptionModel> models)
		{
			foreach (var model in models)
			{
				await SubscriptionService.DeleteSubscriptionAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Subscription> request = BuildDataRequest();

			List<SubscriptionModel> models = [];
			foreach (var range in ranges)
			{
				var items = await SubscriptionService.GetSubscriptionsAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await SubscriptionService.DeleteSubscriptionRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Subscription> BuildDataRequest()
		{
			return new DataRequest<Subscription>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
		}

		private void LogWarning(SubscriptionModel model)
		{
			LogWarning("Subscription", "Delete", "Subscription deleted", $"Subscription {model.SubscriptionID} '{model.FullName}' was deleted.");
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
