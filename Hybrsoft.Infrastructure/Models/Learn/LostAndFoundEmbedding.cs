using Microsoft.Data.SqlTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrsoft.Infrastructure.Models
{
	public class LostAndFoundEmbedding
	{
		public long LostAndFoundEmbeddingID { get; set; }
		public long LostAndFoundID { get; set; }
		public string SearchTerms { get; set; }
		[Column(TypeName = "vector(1536)")]
		public SqlVector<float> Embedding { get; set; }
	}
}
