using Microsoft.Data.SqlTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IEmbeddingService
	{
		int EmbeddingDimension { get; }
		bool IsConfigured { get; }

		Task<SqlVector<float>> GenerateEmbeddingAsync(string input);
		Task<IReadOnlyList<SqlVector<float>>> GenerateEmbeddingsAsync(IEnumerable<string> inputs);
	}
}
