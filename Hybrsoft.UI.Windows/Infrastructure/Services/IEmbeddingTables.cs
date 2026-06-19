using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IEmbeddingTables
	{
		Task PopulateMissingEmbeddingsAsync();
	}
}
