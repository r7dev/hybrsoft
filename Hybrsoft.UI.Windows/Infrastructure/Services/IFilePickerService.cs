using Hybrsoft.UI.Windows.Infrastructure.Commom;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IFilePickerService
	{
		Task<ImagePickerResult> OpenImagePickerAsync();
	}
}
