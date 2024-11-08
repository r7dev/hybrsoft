using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public class UserListViewModel : GenericListViewModel<UserDto>
	{
		public UserListViewModel(IUserService userService, ICommonServices commonServices) : base(commonServices)
		{
			UserService = userService;
		}

		public IUserService UserService { get; }

		public UserListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserListArgs args)
		{
			ViewModelArgs = args ?? UserListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			StartStatusMessage("Loading users...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Users loaded");
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
		}
		public void Subscribe()
		{
			MessageService.Subscribe<UserListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public UserListArgs CreateArgs()
		{
			return new UserListArgs
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		public async Task<bool> RefreshAsync()
		{
			bool isOk = true;

			Items = null;
			ItemsCount = 0;
			SelectedItem = null;

			try
			{
				Items = await GetItemsAsync();
			}
			catch (Exception ex)
			{
				Items = new List<UserDto>();
				StatusError($"Error loading Users: {ex.Message}");
				LogException("Users", "Refresh", ex);
				isOk = false;
			}

			ItemsCount = Items.Count;
			if (!IsMultipleSelection)
			{
				SelectedItem = Items.FirstOrDefault();
			}
			NotifyPropertyChanged(nameof(Title));

			return isOk;
		}

		private async Task<IList<UserDto>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<User> request = BuildDataRequest();
				return await UserService.GetUsersAsync(request);
			}
			return new List<UserDto>();
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<UserDetailsViewModel>(new UserDetailsArgs { UserID = SelectedItem.UserID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<UserDetailsViewModel>(new UserDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<UserDetailsViewModel>(new UserDetailsArgs());
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			StartStatusMessage("Loading users...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Users loaded");
			}
		}

		private DataRequest<User> BuildDataRequest()
		{
			return new DataRequest<User>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemDeleted":
				case "ItemsDeleted":
				case "ItemRangesDeleted":
					await ContextService.RunAsync(async () =>
					{
						await RefreshAsync();
					});
					break;
			}
		}
	}
}
