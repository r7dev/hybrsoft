using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Views;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

namespace Hybrsoft.EnterpriseManager.Services
{
	public partial class NavigationService : INavigationService
	{
		static private readonly ConcurrentDictionary<Type, Type> _viewModelMap = new();

		public NavigationService(IDataServiceFactory dataServiceFactory, IResourceService resourceService)
		{
			Window currentView = ((App)Application.Current).CurrentView;
			var appWindow = AppWindowExtensions.GetAppWindow(currentView);
			MainViewId = (int)appWindow.Id.Value;
			DataServiceFactory = dataServiceFactory;
			ResourceService = resourceService;
		}

		public int MainViewId { get; }

		static public void Register<TViewModel, TView>() where TView : Page
		{
			if (!_viewModelMap.TryAdd(typeof(TViewModel), typeof(TView)))
			{
				throw new InvalidOperationException($"ViewModel already registered '{typeof(TViewModel).FullName}'");
			}
		}

		static public Type GetView<TViewModel>()
		{
			return GetView(typeof(TViewModel));
		}
		static public Type GetView(Type viewModel)
		{
			if (_viewModelMap.TryGetValue(viewModel, out Type view))
			{
				return view;
			}
			throw new InvalidOperationException($"View not registered for ViewModel '{viewModel.FullName}'");
		}

		static public Type GetViewModel(Type view)
		{
			var type = _viewModelMap.Where(r => r.Value == view).Select(r => r.Key).FirstOrDefault();
			return type ?? throw new InvalidOperationException($"View not registered for ViewModel '{view.FullName}'");
		}

		public bool IsMainView => CoreApplication.GetCurrentView().IsMain;

		public Frame Frame { get; private set; }

		public bool CanGoBack => Frame.CanGoBack;

		public void GoBack() => Frame.GoBack();

		public void Initialize(object frame)
		{
			Frame = frame as Frame;
		}

		public bool Navigate<TViewModel>(object parameter = null)
		{
			return Navigate(typeof(TViewModel), parameter);
		}
		public bool Navigate(Type viewModelType, object parameter = null)
		{
			if (Frame == null)
			{
				throw new InvalidOperationException("Navigation frame not initialized.");
			}
			return Frame.Navigate(GetView(viewModelType), parameter);
		}

		public async Task<int> CreateNewViewAsync<TViewModel>(object parameter = null)
		{
			return await CreateNewViewAsync(typeof(TViewModel), parameter);
		}
		public async Task<int> CreateNewViewAsync(Type viewModelType, object parameter = null)
		{
			Window newWindow = new();
			newWindow.SetDefaultIcon();
			newWindow.SetDefaultWindowSize();
			await newWindow.SetWindowPositionToCenterAsync();
			((App)Application.Current).CurrentView = newWindow;

			var frame = new Frame();
			var args = new ShellArgs
			{
				ViewModel = viewModelType,
				Parameter = parameter
			};
			frame.Navigate(typeof(ShellView), args);
			newWindow.Content = frame;

			newWindow.Closed += (s, e) =>
			{
				((App)Application.Current).CurrentView = ((App)Application.Current).MainWindow;
			};
			await Task.Delay(200);
			newWindow.Activate();
			ThemeExtensions.TrySetMicaBackdrop(newWindow, true);

			return (int)newWindow.AppWindow.Id.Value;
		}

		public async Task CloseViewAsync()
		{
			int currentId = ApplicationView.GetForCurrentView().Id;
			await ApplicationViewSwitcher.SwitchAsync(MainViewId, currentId, ApplicationViewSwitchingOptions.ConsolidateViews);
		}

		public IDataServiceFactory DataServiceFactory { get; }
		private static IResourceService ResourceService { get; set; }

		public IEnumerable<NavigationItemDto> GetItems()
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return GetNavigationItemByParentId(dataService.GetNavigationItemByAppType(AppType.EnterpriseManager), null);
		}

		private static IEnumerable<NavigationItemDto> GetNavigationItemByParentId(IList<NavigationItem> items, int? parentId)
		{
			return items.Where(f => f.ParentId == parentId)
				.Select(f => new NavigationItemDto(
					string.IsNullOrEmpty(f.Uid) ? f.Label : ResourceService.GetString(nameof(ResourceFiles.UI), f.Uid),
					f.Icon.Value,
					f.ViewModel,
					f.ParentId,
					[.. GetNavigationItemByParentId(items, f.NavigationItemId)],
					string.IsNullOrEmpty(f.ViewModel) ? null : GetTypeViewModelByName(f.ViewModel)));
		}

		private static Type GetTypeViewModelByName(string view) => view switch
		{
			nameof(DashboardViewModel) => typeof(DashboardViewModel),
			nameof(StudentsViewModel) => typeof(StudentsViewModel),
			nameof(PermissionsViewModel) => typeof(PermissionsViewModel),
			nameof(RolesViewModel) => typeof(RolesViewModel),
			nameof(UsersViewModel) => typeof(UsersViewModel),
			nameof(AppLogsViewModel) => typeof(AppLogsViewModel),
			_ => throw new ArgumentOutOfRangeException(nameof(view), $"Not expected view value: {view}"),
		};
	}
}
