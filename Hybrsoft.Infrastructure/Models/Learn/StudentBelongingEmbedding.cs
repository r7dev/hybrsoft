using Microsoft.Data.SqlTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrsoft.Infrastructure.Models
{
	public class StudentBelongingEmbedding
	{
		public long StudentBelongingEmbeddingID { get; set; }
		public long StudentBelongingID { get; set; }
		public string SearchTerms { get; set; }
		[Column(TypeName = "vector(1536)")]
		public SqlVector<float> Embedding { get; set; }
	}
}
