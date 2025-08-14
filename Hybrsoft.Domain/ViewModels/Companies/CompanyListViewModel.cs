using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class CompanyListViewModel(ICompanyService companyService, ICommonServices commonServices) : GenericListViewModel<CompanyDto>(commonServices)
	{
		public ICompanyService CompanyService { get; } = companyService;

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(CompanyListViewModel), "_Prefix"));
		private bool _hasEditorPermission;

		public CompanyListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(CompanyListArgs args)
		{
			ViewModelArgs = args ?? CompanyListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasEditorPermission = UserPermissionService.HasPermission(Permissions.CompanyEditor);

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyListViewModel), "_LoadingCompanies"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyListViewModel), "_CompaniesLoaded"));
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
			MessageService.Subscribe<CompanyListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public CompanyListArgs CreateArgs()
		{
			return new CompanyListArgs
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
				string resourceKey = string.Concat(nameof(CompanyListViewModel), "_ErrorLoadingCompanies0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Companies", "Refresh", ex);
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

		private async Task<IList<CompanyDto>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Company> request = BuildDataRequest();
				return await CompanyService.GetCompaniesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<CompanyDetailsViewModel>(new CompanyDetailsArgs { CompanyID = SelectedItem.CompanyID });
			}
		}

		public new ICommand NewCommand => new RelayCommand(OnNew, CanNew);
		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<CompanyDetailsViewModel>(new CompanyDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<CompanyDetailsViewModel>(new CompanyDetailsArgs());
			}

			StatusReady();
		}
		private bool CanNew()
		{
			return _hasEditorPermission;
		}

		protected override async void OnRefresh()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyListViewModel), "_LoadingCompanies"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyListViewModel), "_CompaniesLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		public new ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection, CanDeleteSelection);
		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(CompanyListViewModel), "_AreYouSureYouWantToDeleteSelectedCompanies"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(CompanyListViewModel), "_Deleting0Companies");
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
					string resourceKey = string.Concat(nameof(CompanyListViewModel), "_ErrorDeleting0Companies1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("Companies", "Delete", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						string resourceKey = string.Concat(nameof(CompanyListViewModel), "_0CompaniesDeleted");
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

		private async Task DeleteItemsAsync(IEnumerable<CompanyDto> models)
		{
			foreach (var model in models)
			{
				await CompanyService.DeleteCompanyAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Company> request = BuildDataRequest();

			List<CompanyDto> models = [];
			foreach (var range in ranges)
			{
				var items = await CompanyService.GetCompaniesAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await CompanyService.DeleteCompanyRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Company> BuildDataRequest()
		{
			return new DataRequest<Company>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(CompanyDto model)
		{
			LogWarning("Company", "Delete", "Company deleted", $"Company {model.CompanyID} '{model.LegalName}' was deleted.");
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
