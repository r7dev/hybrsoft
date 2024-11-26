using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Extensions;
using System;
using Windows.ApplicationModel.Activation;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	#region ActivationInfo
	public class ActivationInfo
	{
		static public ActivationInfo CreateDefault() => Create<DashboardViewModel>();

		static public ActivationInfo Create<TViewModel>(object entryArgs = null) where TViewModel : ViewModelBase
		{
			return new ActivationInfo
			{
				EntryViewModel = typeof(TViewModel),
				EntryArgs = entryArgs
			};
		}

		public Type EntryViewModel { get; set; }
		public object EntryArgs { get; set; }
	}
	#endregion

	static public class ActivationService
	{
		static public ActivationInfo GetActivationInfo(IActivatedEventArgs args)
		{
			switch (args.Kind)
			{
				case ActivationKind.Protocol:
					return GetProtocolActivationInfo(args as ProtocolActivatedEventArgs);

				case ActivationKind.Launch:
				default:
					return ActivationInfo.CreateDefault();
			}
		}

		private static ActivationInfo GetProtocolActivationInfo(ProtocolActivatedEventArgs args)
		{
			if (args != null)
			{
				switch (args.Uri.AbsolutePath.ToLowerInvariant())
				{
					case "user":
					case "users":
						Guid userID = args.Uri.GetGuidParameter("id");
						if (userID != Guid.Empty)
						{
							return ActivationInfo.Create<UserDetailsViewModel>(new UserDetailsArgs { UserID = userID });
						}
						return ActivationInfo.Create<UsersViewModel>(new UserListArgs());
				}
			}
			return ActivationInfo.CreateDefault();
		}
	}
}
