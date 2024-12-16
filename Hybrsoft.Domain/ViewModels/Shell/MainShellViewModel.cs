using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class MainShellViewModel(ICommonServices commonServices) : ShellViewModel(commonServices)
	{
		private object _selectedItem;
		public object SelectedItem
		{
			get => _selectedItem;
			set => Set(ref _selectedItem, value);
		}

		private bool _isPaneOpen = true;
		public bool IsPaneOpen
		{
			get => _isPaneOpen;
			set => Set(ref _isPaneOpen, value);
		}

		private IEnumerable<NavigationItemDto> _navigationItems;
		public IEnumerable<NavigationItemDto> NavigationItems
		{
			get => _navigationItems;
			set => Set(ref _navigationItems, value);
		}

		public override async Task LoadAsync(ShellArgs args)
		{
			NavigationItems = GetItems().ToArray();
			await UpdateAppLogBadge();
			await base.LoadAsync(args);
		}

		override public void Subscribe()
		{
			MessageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);
			base.Subscribe();
		}

		override public void Unsubscribe()
		{
			base.Unsubscribe();
		}

		public override void Unload()
		{
			base.Unload();
		}

		public async void NavigateTo(Type viewModel)
		{
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
				case "AppLogsViewModel":
					NavigationService.Navigate(viewModel, new AppLogListArgs());
					await LogService.MarkAllAsReadAsync();
					await UpdateAppLogBadge();
					break;
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

		private async void OnLogServiceMessage(ILogService logService, string message, AppLog log)
		{
			if (message == "LogAdded")
			{
				await ContextService.RunAsync(async () =>
				{
					await UpdateAppLogBadge();
				});
			}
		}

		private async Task UpdateAppLogBadge()
		{
			int count = await LogService.GetLogsCountAsync(new DataRequest<AppLog> { Where = r => !r.IsRead });
			var appLogsItem = NavigationItems.Where(f => f.Tag == "AppLogs").FirstOrDefault();
			appLogsItem.Badge = count > 0 ?
				new Microsoft.UI.Xaml.Controls.InfoBadge
				{
					Style = (Style)Application.Current.Resources["CriticalValueInfoBadgeStyle"],
					Value = count
				}
				:
				null;
		}
	}
}
