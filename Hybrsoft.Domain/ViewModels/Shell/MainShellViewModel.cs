using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class MainShellViewModel(ICommonServices commonServices) : ShellViewModel(commonServices)
	{
		private IEnumerable<NavigationItemDto> _navigationItems;
		public IEnumerable<NavigationItemDto> NavigationItems
		{
			get => _navigationItems;
			set => Set(ref _navigationItems, value);
		}

		public override async Task LoadAsync(ShellArgs args)
		{
			NavigationItems = GetItems().ToArray();
			//await UpdateAppLogBadge();
			await base.LoadAsync(args);
		}

		override public void Subscribe()
		{
			//MessageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);
			base.Subscribe();
		}

		public async void NavigateTo(Type viewModel)
		{
			await Task.Delay(100);
			switch (viewModel.Name)
			{
				case "DashboardViewModel":
					NavigationService.Navigate(viewModel);
					break;
				case "UsersViewModel":
					NavigationService.Navigate(viewModel, new UserListArgs());
					break;
				//case "OrdersViewModel":
				//	NavigationService.Navigate(viewModel, new OrderListArgs());
				//	break;
				//case "ProductsViewModel":
				//	NavigationService.Navigate(viewModel, new ProductListArgs());
				//	break;
				//case "AppLogsViewModel":
				//	NavigationService.Navigate(viewModel, new AppLogListArgs());
				//	await LogService.MarkAllAsReadAsync();
				//	await UpdateAppLogBadge();
				//	break;
				//case "SettingsViewModel":
				//	NavigationService.Navigate(viewModel, new SettingsArgs());
				//	break;
				default:
					throw new NotImplementedException();
			}
		}

		private IEnumerable<NavigationItemDto> GetItems()
		{
			return NavigationService.GetItems();
		}
	}
}
