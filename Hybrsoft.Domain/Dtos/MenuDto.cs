namespace Hybrsoft.Domain.Dtos
{
	public class MenuDto
	{
		public int MenuId { get; set; }
		public string Nome { get; set; }
		public int? Icone { get; set; }
		public int? SuperiorId { get; set; }
		public virtual MenuDto Superior { get; set; }
	}
}
