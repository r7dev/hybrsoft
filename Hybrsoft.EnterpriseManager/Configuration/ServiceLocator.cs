﻿using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Services;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.Infrastructure.LogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Concurrent;
using Windows.UI.ViewManagement;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	public partial class ServiceLocator : IDisposable
	{
		static private readonly ConcurrentDictionary<int, ServiceLocator> _serviceLocators = new();

		static private ServiceProvider _rootServiceProvider = null;

		static public void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ISettingsService, SettingsService>();
			serviceCollection.AddSingleton<IDataServiceFactory, DataServiceFactory>();
			serviceCollection.AddSingleton<IPermissionService, PermissionService>();
			serviceCollection.AddSingleton<IUserService, UserService>();

			serviceCollection.AddSingleton<IMessageService, MessageService>();
			serviceCollection.AddSingleton<IDialogService, DialogService>();
			serviceCollection.AddSingleton<ILogService, LogService>();
			serviceCollection.AddSingleton<ILoginService, LoginService>();

			serviceCollection.AddScoped<IContextService, ContextService>();
			serviceCollection.AddScoped<INavigationService, NavigationService>();
			serviceCollection.AddScoped<ICommonServices, CommonServices>();

			serviceCollection.AddTransient<LoginViewModel>();

			serviceCollection.AddTransient<ShellViewModel>();
			serviceCollection.AddTransient<MainShellViewModel>();

			serviceCollection.AddTransient<DashboardViewModel>();

			serviceCollection.AddTransient<PermissionsViewModel>();
			serviceCollection.AddTransient<PermissionDetailsViewModel>();
			serviceCollection.AddTransient<UsersViewModel>();
			serviceCollection.AddTransient<UserDetailsViewModel>();

			serviceCollection.AddTransient<AppLogsViewModel>();

			_rootServiceProvider = serviceCollection.BuildServiceProvider();
		}

		static public ServiceLocator Current
		{
			get
			{
				Window currentView = ((App)Application.Current).CurrentView;
				var appWindow = AppWindowExtensions.GetAppWindow(currentView);
				int currentViewId = (int)appWindow.Id.Value;
				return _serviceLocators.GetOrAdd(currentViewId, key => new ServiceLocator());
			}
		}

		static public void DisposeCurrent()
		{
			int currentViewId = ApplicationView.GetForCurrentView().Id;
			if (_serviceLocators.TryRemove(currentViewId, out ServiceLocator current))
			{
				current.Dispose();
			}
		}

		private readonly IServiceScope _serviceScope = null;

		private ServiceLocator()
		{
			_serviceScope = _rootServiceProvider.CreateScope();
		}

		public T GetService<T>()
		{
			return GetService<T>(true);
		}

		public T GetService<T>(bool isRequired)
		{
			if (isRequired)
			{
				return _serviceScope.ServiceProvider.GetRequiredService<T>();
			}
			return _serviceScope.ServiceProvider.GetService<T>();
		}

		#region Dispose
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_serviceScope?.Dispose();
			}
		}
		#endregion
	}
}
