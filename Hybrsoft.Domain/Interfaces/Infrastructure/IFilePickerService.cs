using Hybrsoft.Domain.Infrastructure.Commom;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface IFilePickerService
	{
		Task<ImagePickerResult> OpenImagePickerAsync();
	}
}
