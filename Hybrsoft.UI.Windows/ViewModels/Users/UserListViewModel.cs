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
	public partial class UserListViewModel(IUserService userService, ICommonServices commonServices) : GenericListViewModel<UserModel>(commonServices)
	{
		public IUserService UserService { get; } = userService;

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(UserListViewModel), "_Prefix"));

		public UserListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserListArgs args)
		{
			ViewModelArgs = args ?? UserListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserListViewModel), "_LoadingUsers"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserListViewModel), "_UsersLoaded"));
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
			MessageService.Subscribe<UserListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public UserListArgs CreateArgs()
		{
			return new UserListArgs
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
				string resourceKey = string.Concat(nameof(UserListViewModel), "_ErrorLoadingUsers0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError($"Error loading Users: {ex.Message}");
				LogException("Users", "Refresh", ex);
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

		private async Task<IList<UserModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<User> request = BuildDataRequest();
				return await UserService.GetUsersAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<UserDetailsViewModel>(new UserDetailsArgs { UserID = SelectedItem.UserID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<UserDetailsViewModel>(new UserDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<UserDetailsViewModel>(new UserDetailsArgs());
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserListViewModel), "_LoadingUsers"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserListViewModel), "_UsersLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(UserListViewModel), "_AreYouSureYouWantToDeleteSelectedUsers"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, "Are you sure you want to delete selected users?", delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(UserListViewModel), "_Deleting0Users");
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
					string resourceKey = string.Concat(nameof(UserListViewModel), "_ErrorDeleting0Users1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("Users", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(UserListViewModel), "_0UsersDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage(message, LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<UserModel> models)
		{
			foreach (var model in models)
			{
				await UserService.DeleteUserAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<User> request = BuildDataRequest();

			List<UserModel> models = [];
			foreach (var range in ranges)
			{
				var items = await UserService.GetUsersAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await UserService.DeleteUserRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<User> BuildDataRequest()
		{
			return new DataRequest<User>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(UserModel model)
		{
			LogWarning("User", "Delete", "User deleted", $"User {model.UserID} '{model.FullName}' was deleted.");
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
