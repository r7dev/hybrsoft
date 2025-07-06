using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class CompanyUserListViewModel(ICompanyUserService companyUserService, ICommonServices commonServices) : GenericListViewModel<CompanyUserDto>(commonServices)
	{
		public ICompanyUserService CompanyUserService { get; } = companyUserService;

		public CompanyUserListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(CompanyUserListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? CompanyUserListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

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

		private async Task<IList<CompanyUserDto>> GetItemsAsync()
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

		private async Task DeleteItemsAsync(IEnumerable<CompanyUserDto> models)
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

			List<CompanyUserDto> models = [];
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

		private void LogWarning(CompanyUserDto model)
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
