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
	public partial class RoleListViewModel(IRoleService roleService, ICommonServices commonServices) : GenericListViewModel<RoleModel>(commonServices)
	{
		public IRoleService RoleService { get; } = roleService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<RoleListViewModel>(ResourceFiles.InfoMessages, "LoadingRoles");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<RoleListViewModel>(ResourceFiles.InfoMessages, "RolesLoaded");
		public string Prefix => ResourceService.GetString<RoleListViewModel>(ResourceFiles.UI, "Prefix");

		public RoleListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RoleListArgs args)
		{
			ViewModelArgs = args ?? RoleListArgs.CreateEmpty();
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
			MessageService.Subscribe<RoleListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public RoleListArgs CreateArgs()
		{
			return new RoleListArgs
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
				string title = ResourceService.GetString(ResourceFiles.Errors, "LoadFailed");
				string message = ResourceService.GetString<RoleListViewModel>(ResourceFiles.Errors, "ErrorLoadingRoles0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Roles", "Refresh", ex);
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

		private async Task<IList<RoleModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Role> request = BuildDataRequest();
				return await RoleService.GetRolesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<RoleDetailsViewModel>(new RoleDetailsArgs { RoleID = SelectedItem.RoleID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<RoleDetailsViewModel>(new RoleDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<RoleDetailsViewModel>(new RoleDetailsArgs());
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
			string content = ResourceService.GetString<RoleListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedRoles");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<RoleListViewModel>(ResourceFiles.InfoMessages, "Deleting0Roles");
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
					string message = ResourceService.GetString<RoleListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Roles1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("Roles", "Delete", ex);
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
						string message = ResourceService.GetString<RoleListViewModel>(ResourceFiles.InfoMessages, "0RolesDeleted");
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

		private async Task DeleteItemsAsync(IEnumerable<RoleModel> models)
		{
			foreach (var model in models)
			{
				await RoleService.DeleteRoleAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Role> request = BuildDataRequest();

			List<RoleModel> models = [];
			foreach (var range in ranges)
			{
				var items = await RoleService.GetRolesAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await RoleService.DeleteRoleRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Role> BuildDataRequest()
		{
			return new DataRequest<Role>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(RoleModel model)
		{
			LogWarning("Role", "Delete", "Role deleted", $"Role {model.RoleID} '{model.Name}' was deleted.");
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
