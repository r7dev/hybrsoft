using Microsoft.Data.SqlTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrsoft.Infrastructure.Models
{
	public class AppLogEmbedding
	{
		public long AppLogEmbeddingID { get; set; }
		[Column(TypeName = "vector(1536)")]
		public SqlVector<float> Embedding { get; set; }
	}
}
