using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public class MainShellViewModel : ShellViewModel
	{
		public MainShellViewModel(ICommonServices commonServices) : base(commonServices)
		{
		}

		private IEnumerable<NavigationItem> _items;
		public IEnumerable<NavigationItem> Items
		{
			get => _items;
			set => Set(ref _items, value);
		}

		public override async Task LoadAsync(ShellArgs args)
		{
			Items = GetItems().ToArray();
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

		private IEnumerable<NavigationItem> GetItems()
		{
			return NavigationService.GetItems();
		}
	}
}
