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
	public partial class CompanyListViewModel(ICompanyService companyService, ICommonServices commonServices) : GenericListViewModel<CompanyModel>(commonServices)
	{
		public ICompanyService CompanyService { get; } = companyService;

		private bool _hasEditorPermission;
		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<CompanyListViewModel>(ResourceFiles.InfoMessages, "LoadingCompanies");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<CompanyListViewModel>(ResourceFiles.InfoMessages, "CompaniesLoaded");
		public string Prefix => ResourceService.GetString<CompanyListViewModel>(ResourceFiles.UI, "Prefix");

		public CompanyListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(CompanyListArgs args)
		{
			ViewModelArgs = args ?? CompanyListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasEditorPermission = AuthorizationService.HasPermission(Permissions.CompanyEditor);

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
				string title = ResourceService.GetString(ResourceFiles.Errors, "LoadFailed");
				string message = ResourceService.GetString<CompanyListViewModel>(ResourceFiles.Errors, "ErrorLoadingCompanies0");
				StatusError(title, string.Format(message, ex.Message));
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

		private async Task<IList<CompanyModel>> GetItemsAsync()
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
			string content = ResourceService.GetString<CompanyListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedCompanies");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<CompanyListViewModel>(ResourceFiles.InfoMessages, "Deleting0Companies");
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
					string message = ResourceService.GetString<CompanyListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Companies1");
					StatusError(title, string.Format(message, count, ex.Message));
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
						string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
						string message = ResourceService.GetString<CompanyListViewModel>(ResourceFiles.InfoMessages, "0CompaniesDeleted");
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
		private bool CanDeleteSelection()
		{
			return _hasEditorPermission;
		}

		private async Task DeleteItemsAsync(IEnumerable<CompanyModel> models)
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

			List<CompanyModel> models = [];
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

		private void LogWarning(CompanyModel model)
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
