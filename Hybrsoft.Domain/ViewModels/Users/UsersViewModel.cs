using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class UsersViewModel : ViewModelBase
	{
		public UsersViewModel(IUserService userService, ICommonServices commonServices) : base(commonServices)
		{
			UserService = userService;

			UserList = new UserListViewModel(UserService, commonServices);
		}

		public IUserService UserService { get; }

		public UserListViewModel UserList { get; set; }

		public async Task LoadAsync(UserListArgs args)
		{
			await UserList.LoadAsync(args);
		}

		public void Unload()
		{
			UserList.Unload();
		}

		public void Subscribe()
		{
			UserList.Subscribe();
		}
		public void Unsubscribe()
		{
			UserList.Unsubscribe();
		}
	}
}
