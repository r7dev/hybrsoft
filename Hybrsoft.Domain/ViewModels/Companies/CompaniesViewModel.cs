using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class CompaniesViewModel : ViewModelBase
	{
		public CompaniesViewModel(ICompanyService companyService, ICompanyUserService companyUserService, ICommonServices commonServices) : base(commonServices)
		{
			CompanyService = companyService;
			CompanyList = new CompanyListViewModel(CompanyService, commonServices);
			CompanyDetails = new CompanyDetailsViewModel(CompanyService, commonServices);
			CompanyUserList = new CompanyUserListViewModel(companyUserService, commonServices);
		}

		public ICompanyService CompanyService { get; }

		public CompanyListViewModel CompanyList { get; set; }

		public CompanyDetailsViewModel CompanyDetails { get; set; }

		public CompanyUserListViewModel CompanyUserList { get; set; }

		public async Task LoadAsync(CompanyListArgs args)
		{
			await CompanyList.LoadAsync(args);
		}

		public void Unload()
		{
			CompanyDetails.CancelEdit();
			CompanyList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<CompanyListViewModel>(this, OnMessage);
			CompanyList.Subscribe();
			CompanyDetails.Subscribe();
			CompanyUserList.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			CompanyList.Unsubscribe();
			CompanyDetails.Unsubscribe();
			CompanyUserList.Unsubscribe();
		}

		private async void OnMessage(CompanyListViewModel viewModel, string message, object args)
		{
			if (viewModel == CompanyList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (CompanyDetails.IsEditMode)
			{
				StatusReady();
				CompanyDetails.CancelEdit();
			}
			CompanyUserList.IsMultipleSelection = false;
			var selected = CompanyList.SelectedItem;
			if (!CompanyUserList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
					await PopulateCompanyUsers(selected);
				}
			}
			CompanyDetails.Item = selected;
		}

		private async Task PopulateDetails(CompanyDto selected)
		{
			try
			{
				var model = await CompanyService.GetCompanyAsync(selected.CompanyID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Companies", "Load Details", ex);
			}
		}

		private async Task PopulateCompanyUsers(CompanyDto selectedItem)
		{
			try
			{
				if (selectedItem != null)
				{
					await CompanyUserList.LoadAsync(new CompanyUserListArgs { CompanyID = selectedItem.CompanyID }, silent: true);
				}
			}
			catch (Exception ex)
			{
				LogException("Companies", "Load Users in the Company", ex);
			}
		}
	}
}
