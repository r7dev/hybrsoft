using Hybrsoft.UI.Windows.Infrastructure.Commom;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces.Infrastructure
{
	public interface IFilePickerService
	{
		Task<ImagePickerResult> OpenImagePickerAsync();
	}
}
