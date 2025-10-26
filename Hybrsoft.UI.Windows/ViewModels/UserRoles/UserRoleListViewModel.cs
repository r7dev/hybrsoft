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
	public partial class UserRoleListViewModel(IUserRoleService userRoleService, ICommonServices commonServices) : GenericListViewModel<UserRoleModel>(commonServices)
	{
		public IUserRoleService UserRoleService { get; } = userRoleService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.InfoMessages, "LoadingUserRoles");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.InfoMessages, "UserRolesLoaded");

		public UserRoleListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserRoleListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? UserRoleListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			if (silent)
			{
				await RefreshAsync();
			}
			else
			{
				await RefreshWithStatusAsync();
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
			MessageService.Subscribe<UserRoleListViewModel>(this, OnMessage);
			MessageService.Subscribe<UserRoleDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public UserRoleListArgs CreateArgs()
		{
			return new UserRoleListArgs
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys,
				UserId = ViewModelArgs.UserId
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
				string message = ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.Errors, "ErrorLoadingUserRoles0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("UserRoles", "Refresh", ex);
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

		private async Task<IList<UserRoleModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<UserRole> request = BuildDataRequest();
				return await UserRoleService.GetUserRolesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserRoleId = SelectedItem.UserRoleID, UserId = SelectedItem.UserID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserId = ViewModelArgs.UserId });
			}
			else
			{
				NavigationService.Navigate<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserId = ViewModelArgs.UserId });
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
			string content = ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedUserRoles");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.InfoMessages, "Deleting0UserRoles");
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
					string message = ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.Errors, "ErrorDeleting0UserRoles1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("UserRoles", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<UserRoleListViewModel>(ResourceFiles.InfoMessages, "0UserRolesDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<UserRoleModel> models)
		{
			foreach (var model in models)
			{
				await UserRoleService.DeleteUserRoleAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<UserRole> request = BuildDataRequest();

			List<UserRoleModel> models = [];
			foreach (var range in ranges)
			{
				var items = await UserRoleService.GetUserRolesAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await UserRoleService.DeleteUserRoleRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<UserRole> BuildDataRequest()
		{
			var request = new DataRequest<UserRole>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
			if (ViewModelArgs.UserId > 0)
			{
				request.Where = (r) => r.UserID == ViewModelArgs.UserId;
			}
			return request;
		}

		private void LogWarning(UserRoleModel model)
		{
			LogWarning("UserRole", "Delete", "User Role deleted", $"User Role #{model.UserID}, '{model.Role.Name}' was deleted.");
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemChanged":
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
