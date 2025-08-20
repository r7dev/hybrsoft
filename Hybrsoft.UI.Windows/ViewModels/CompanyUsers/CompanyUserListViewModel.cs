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
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class CompanyUserListViewModel(ICompanyUserService companyUserService, ICommonServices commonServices) : GenericListViewModel<CompanyUserModel>(commonServices)
	{
		public ICompanyUserService CompanyUserService { get; } = companyUserService;

		private bool _hasEditorPermission;

		public CompanyUserListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(CompanyUserListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? CompanyUserListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasEditorPermission = AuthorizationService.HasPermission(Permissions.CompanyEditor);

			if (silent)
			{
				await RefreshAsync();
			}
			else
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyUserListViewModel), "_LoadingCompanyUsers"));
				StartStatusMessage(startMessage);
				if (await RefreshAsync())
				{
					string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyUserListViewModel), "_CompanyUsersLoaded"));
					EndStatusMessage(endMessage);
				}
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
			MessageService.Subscribe<CompanyUserListViewModel>(this, OnMessage);
			MessageService.Subscribe<CompanyUserDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public CompanyUserListArgs CreateArgs()
		{
			return new CompanyUserListArgs
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc,
				CompanyID = ViewModelArgs.CompanyID
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
				string resourceKey = string.Concat(nameof(CompanyUserListViewModel), "_ErrorLoadingCompanyUsers0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("CompanyUsers", "Refresh", ex);
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

		private async Task<IList<CompanyUserModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<CompanyUser> request = BuildDataRequest();
				return await CompanyUserService.GetCompanyUsersAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<CompanyUserDetailsViewModel>(new CompanyUserDetailsArgs { CompanyUserID = SelectedItem.CompanyUserID, CompanyID = SelectedItem.CompanyID });
			}
		}

		public new ICommand NewCommand => new RelayCommand(OnNew, CanNew);
		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<CompanyUserDetailsViewModel>(new CompanyUserDetailsArgs { CompanyID = ViewModelArgs.CompanyID });
			}
			else
			{
				NavigationService.Navigate<CompanyUserDetailsViewModel>(new CompanyUserDetailsArgs { CompanyID = ViewModelArgs.CompanyID });
			}

			StatusReady();
		}
		private bool CanNew()
		{
			return _hasEditorPermission;
		}

		protected override async void OnRefresh()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyUserListViewModel), "_LoadingCompanyUsers"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyUserListViewModel), "_CompanyUsersLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		public new ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection, CanDeleteSelection);
		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(CompanyUserListViewModel), "_AreYouSureYouWantToDeleteSelectedCompanyUsers"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(CompanyUserListViewModel), "_Deleting0CompanyUsers");
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
					string resourceKey = string.Concat(nameof(CompanyUserListViewModel), "_ErrorDeleting0CompanyUsers1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("CompanyUsers", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(CompanyUserListViewModel), "_0CompanyUsersDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage($"{count} Company Users deleted", LogType.Warning);
				}
			}
		}
		private bool CanDeleteSelection()
		{
			return _hasEditorPermission;
		}

		private async Task DeleteItemsAsync(IEnumerable<CompanyUserModel> models)
		{
			foreach (var model in models)
			{
				await CompanyUserService.DeleteCompanyUserAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<CompanyUser> request = BuildDataRequest();

			List<CompanyUserModel> models = [];
			foreach (var range in ranges)
			{
				var items = await CompanyUserService.GetCompanyUsersAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await CompanyUserService.DeleteCompanyUserRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<CompanyUser> BuildDataRequest()
		{
			var request = new DataRequest<CompanyUser>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
			if (ViewModelArgs.CompanyID > 0)
			{
				request.Where = (r) => r.CompanyID == ViewModelArgs.CompanyID;
			}
			return request;
		}

		private void LogWarning(CompanyUserModel model)
		{
			LogWarning("CompanyUser", "Delete", "Company User deleted", $"Company User #{model.CompanyID}, '{model.User.FullName}' was deleted.");
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
