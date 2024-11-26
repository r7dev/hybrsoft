using Hybrsoft.Domain.Infrastructure.Commom;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ILoginService
	{
		bool IsAuthenticated { get; set; }

		Task<bool> SignInWithPasswordAsync(string userName, string password);

		bool IsWindowsHelloEnabled(string userName);
		Task TrySetupWindowsHelloAsync(string userName);
		Task<Result> SignInWithWindowsHelloAsync();

		void Logoff();
	}
}
