namespace Hybrsoft.DTOs
{
	public class UserRequest
	{
		public required string Username { get; set; }
		public required string Password { get; set; }
		public bool IsEncrypted { get; set; }
	}
}
