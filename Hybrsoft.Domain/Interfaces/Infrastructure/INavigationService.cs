using Hybrsoft.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface INavigationService
	{
		bool IsMainView { get; }
		bool CanGoBack { get; }

		void Initialize(object frame);

		bool Navigate<TViewModel>(object parameter = null);
		bool Navigate(Type viewModelType, object parameter = null);

		Task<int> CreateNewViewAsync<TViewModel>(object parameter = null);
		Task<int> CreateNewViewAsync(Type viewModelType, object parameter = null);

		void GoBack();

		Task CloseViewAsync();

		IEnumerable<NavigationItem> GetItems();
	}
}
