using Hybrsoft.Domain.Services;
using Hybrsoft.FoundationAPI.Services.DataServiceFactory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hybrsoft.FoundationAPI.Services
{
	public class AuthService(IDataServiceFactory dataServiceFactory,
		ISecurityService securityService,
		IConfiguration configuration) : IAuthService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly ISecurityService _securityService = securityService;
		private readonly IConfiguration _configuration = configuration;

		public async Task<string> AuthenticateAsync(string username, string password, bool isEncrypted)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				throw new UnauthorizedAccessException("Username or password cannot be empty.");
			}

			var dataService = _dataServiceFactory.CreateDataService();
			var user = await dataService.GetUserByEmailAsync(username) ?? throw new UnauthorizedAccessException("User not found.");
			if (isEncrypted)
			{
				var parts = user.Password.Split('-');
				if (parts.Length != 2)
				{
					throw new UnauthorizedAccessException("Invalid password format.");
				}
				if (parts[0] != password)
				{
					throw new UnauthorizedAccessException("Invalid username or password.");
				}
			}
			else if (!_securityService.VerifyHashedPassword(user.Password, password))
			{
				throw new UnauthorizedAccessException("Invalid username or password.");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Email)]),
				Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationMinutes"])),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
