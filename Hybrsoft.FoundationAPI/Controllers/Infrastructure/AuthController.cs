using Hybrsoft.DTOs;
using Hybrsoft.FoundationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hybrsoft.FoundationAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController(IAuthService authService) : ControllerBase
	{
		private readonly IAuthService _authService = authService;

		[HttpPost("login")]
		public async Task<IActionResult> LoginAsync([FromBody] UserRequest user)
		{
			try
			{
				var token = await _authService.AuthenticateAsync(user.Username, user.Password, user.IsEncrypted);
				if (token == null)
					return Unauthorized();

				return Ok(new { Token = token });
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
