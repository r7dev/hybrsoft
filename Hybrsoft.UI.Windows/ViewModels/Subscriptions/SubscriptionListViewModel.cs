using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
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
		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(SubscriptionListViewModel), "_Prefix"));

		public SubscriptionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(SubscriptionListArgs args)
		{
			ViewModelArgs = args ?? SubscriptionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasEditorPermission = UserPermissionService.HasPermission(Permissions.SubscriptionEditor);

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(SubscriptionListViewModel), "_LoadingSubscriptions"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(SubscriptionListViewModel), "_SubscriptionsLoaded"));
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
				string resourceKey = string.Concat(nameof(SubscriptionListViewModel), "_ErrorLoadingSubscriptions0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(SubscriptionListViewModel), "_LoadingSubscriptions"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(SubscriptionListViewModel), "_SubscriptionsLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		public new ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection, CanDeleteSelection);
		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(SubscriptionListViewModel), "_AreYouSureYouWantToDeleteSelectedSubscriptions"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(SubscriptionListViewModel), "_Deleting0Subscriptions");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
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
					string resourceKey = string.Concat(nameof(SubscriptionListViewModel), "_ErrorDeleting0Subscriptions1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
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
						string resourceKey = string.Concat(nameof(SubscriptionListViewModel), "_0SubscriptionsDeleted");
						string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
						string message = string.Format(resourceValue, count);
						EndStatusMessage(message, LogType.Warning);
					}
				}
				else
				{
					string message = ResourceService.GetString(nameof(ResourceFiles.Errors), "DeleteNotAllowed");
					StatusError(message);
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
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
