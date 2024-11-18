﻿using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

namespace Hybrsoft.EnterpriseManager.Services
{
	public partial class NavigationService : INavigationService
	{
		static private readonly ConcurrentDictionary<Type, Type> _viewModelMap = new ConcurrentDictionary<Type, Type>();

		public NavigationService(IDataServiceFactory dataServiceFactory)
		{
			MainViewId = -4654874;//ApplicationView.GetForCurrentView().Id;
			DataServiceFactory = dataServiceFactory;
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
			if (type == null)
			{
				throw new InvalidOperationException($"View not registered for ViewModel '{view.FullName}'");
			}
			return type;
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
			AppWindowExtensions.SetDefaultIcon(newWindow.AppWindow);
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

		public IEnumerable<NavigationItem> GetItems()
		{
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				return GetItemsBySuperiorId(dataService.GetMenus(), null);
			}
		}

		private IEnumerable<NavigationItem> GetItemsBySuperiorId(IList<Hybrsoft.Infrastructure.Models.Menu> items, int? superiorId)
		{
			return items.Where(f => f.SuperiorId == superiorId)
				.Select(f => new NavigationItem(
					f.Icone.Value,
					f.Nome,
					f.SuperiorId,
					new ObservableCollection<NavigationItem>(GetItemsBySuperiorId(items, f.MenuId)),
					GetTypeViewModelByName(f.Nome)));
		}

		private Type GetTypeViewModelByName(string name)
		{
			if (name == "Usuários")
			{
				return typeof(UsersViewModel);
			}
			return null;
		}
	}
}
