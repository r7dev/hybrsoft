using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class DashboardViewModel(ICommonServices commonServices) : ViewModelBase(commonServices)
	{
	}
}
