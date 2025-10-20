using Hybrsoft.UI.Windows.Infrastructure.Common;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IFilePickerService
	{
		Task<ImagePickerResult> OpenImagePickerAsync();
	}
}
