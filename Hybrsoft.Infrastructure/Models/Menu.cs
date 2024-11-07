namespace Hybrsoft.Infrastructure.Models
{
	public class Menu
	{
		public int MenuId { get; set; }
		public string Nome { get; set; }
		public int? Icone { get; set; }
		public int? SuperiorId { get; set; }
		public virtual Menu Superior { get; set; }
	}
}
