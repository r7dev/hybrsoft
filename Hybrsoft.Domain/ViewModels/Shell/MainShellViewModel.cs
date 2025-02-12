using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			await base.LoadAsync(args);
			await UpdateAppLogBadge();
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
				case nameof(DashboardViewModel):
					NavigationService.Navigate(viewModel);
					break;
				case nameof(PermissionsViewModel):
					NavigationService.Navigate(viewModel, new PermissionListArgs());
					break;
				case nameof(RolesViewModel):
					NavigationService.Navigate(viewModel, new RoleListArgs());
					break;
				case nameof(UsersViewModel):
					NavigationService.Navigate(viewModel, new UserListArgs());
					break;
				case nameof(AppLogsViewModel):
					NavigationService.Navigate(viewModel, new AppLogListArgs());
					await LogService.MarkAllAsReadAsync();
					await UpdateAppLogBadge();
					break;
				case nameof(SettingsViewModel):
					NavigationService.Navigate(viewModel, new SettingsArgs());
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(viewModel), $"Not expected view value: {viewModel.Name}");
			}
		}

		private IEnumerable<NavigationItemDto> GetItems()
		{
			return NavigationService.GetItems().Where(HasUserPermission);
		}

		private bool HasUserPermission(NavigationItemDto item)
		{
			if (item.ViewModel is null && item.Children != null)
			{
				if (item.Children.Count == 0)
				{
					return true;
				}
				var validChildren = item.Children.Where(c => HasUserPermission(c));
				item.Children = new ObservableCollection<NavigationItemDto>(validChildren);
				return item.Children.Any();
			}
			var IsSecurityAdministration = UserPermissionService.HasPermission(Permissions.SecurityAdministration);
			return (item.ViewModel == typeof(DashboardViewModel)
				|| item.ViewModel == typeof(PermissionsViewModel)
				|| item.ViewModel == typeof(RolesViewModel)
				|| item.ViewModel == typeof(UsersViewModel)
				|| item.ViewModel == typeof(AppLogsViewModel))
				&& IsSecurityAdministration;
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
			var request = new DataRequest<AppLog> { Where = r => !r.IsRead && r.AppType == AppType.EnterpriseManager };
			int count = await LogService.GetLogsCountAsync(request);
			var appLogsItem = NavigationItems.FirstOrDefault(f => f.Tag == nameof(AppLogsViewModel));
			if (appLogsItem != null)
			{
				appLogsItem.Badge = count > 0
					? new Microsoft.UI.Xaml.Controls.InfoBadge
					{
						Style = (Style)Application.Current.Resources["CriticalValueInfoBadgeStyle"],
						Value = count
					}
					: null;
			}
		}
	}
}
