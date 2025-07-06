using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class CompanyDetailsWithUsersViewModel(ICompanyService companyService, ICompanyUserService companyUserService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public CompanyDetailsViewModel CompanyDetails { get; set; } = new CompanyDetailsViewModel(companyService, commonServices);
		public CompanyUserListViewModel CompanyUserList { get; set; } = new CompanyUserListViewModel(companyUserService, commonServices);

		public async Task LoadAsync(CompanyDetailsArgs args)
		{
			await CompanyDetails.LoadAsync(args);

			long id = args?.CompanyID ?? 0;
			if (id > 0)
			{
				await CompanyUserList.LoadAsync(new CompanyUserListArgs { CompanyID = args.CompanyID });
			}
			else
			{
				await CompanyUserList.LoadAsync(new CompanyUserListArgs { IsEmpty = true }, silent: true);
			}
		}
		public void Unload()
		{
			CompanyDetails.CancelEdit();
			CompanyDetails.Unload();
			CompanyUserList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<CompanyDetailsViewModel, CompanyDto>(this, OnMessage);
			CompanyDetails.Subscribe();
			CompanyUserList.Subscribe();
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			CompanyDetails.Unsubscribe();
			CompanyUserList.Unsubscribe();
		}

		private async void OnMessage(CompanyDetailsViewModel viewModel, string message, CompanyDto model)
		{
			if (viewModel == CompanyDetails && (message == "NewItemSaved" || message == "ItemChanged"))
			{
				await CompanyUserList.LoadAsync(new CompanyUserListArgs { CompanyID = model.CompanyID });
			}
		}
	}
}
