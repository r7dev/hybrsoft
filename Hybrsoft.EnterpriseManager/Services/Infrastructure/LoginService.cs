using Hybrsoft.Domain.Services;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class LoginService(IUserService userService,
		ISecurityService securityService,
		ISettingsService settingsService,
		ICommonServices commonServices) : ILoginService
	{
		private readonly IUserService _userService = userService;
		private readonly ISecurityService _securityService = securityService;
		private readonly ISettingsService _settingsService = settingsService;
		private readonly ICommonServices _commonServices = commonServices;

		public bool IsAuthenticated { get; set; } = false;

		public async Task<bool> IsWindowsHelloEnabledAsync(string userName)
		{
			bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
			if (keyCredentialAvailable && !string.IsNullOrEmpty(userName))
			{
				if (userName.Equals(AppSettings.Current.UserName, StringComparison.OrdinalIgnoreCase))
				{
					return AppSettings.Current.WindowsHelloPublicKeyHint != null;
				}
			}
			return false;
		}

		public async Task<Result> SignInWithPasswordAsync(string userName, string password)
		{
			UserModel user = await _userService.GetUserByEmailAsync(userName, true);
			bool isUserAuthenticated = user != null && _securityService.VerifyHashedPassword(user.Password, password);
			AppSettings.Current.UserID = isUserAuthenticated ? user.UserID : default;
			AppSettings.Current.UserName = isUserAuthenticated ? userName : default;
			_settingsService.UserFirstName = isUserAuthenticated ? user?.FirstName : default;
			_settingsService.UserLastName = isUserAuthenticated ? user?.LastName : default;
			UpdateAuthenticationStatus(isUserAuthenticated);
			if (isUserAuthenticated)
				return Result.Ok();

			string message = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "LoginError");
			string description = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "PleaseEnterValidUsernameAndPassword");
			return Result.Error(message, description);
		}

		public async Task<Result> SignInWithWindowsHelloAsync()
		{
			string userName = AppSettings.Current.UserName;
			if (await IsWindowsHelloEnabledAsync(userName))
			{
				var retrieveResult = await KeyCredentialManager.OpenAsync(userName);
				if (retrieveResult.Status == KeyCredentialStatus.Success)
				{
					var credential = retrieveResult.Credential;
					var challengeBuffer = CryptographicBuffer.DecodeFromBase64String("challenge");
					var result = await credential.RequestSignAsync(challengeBuffer);
					if (result.Status == KeyCredentialStatus.Success)
					{
						UpdateAuthenticationStatus(true);
						return Result.Ok();
					}
					string description = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "CannotSignInWithWindowsHello");
					return Result.Error("Windows Hello", $"{description} {result.Status}");
				}
				string description2 = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "CannotSignInWithWindowsHello");
				return Result.Error("Windows Hello", $"{description2} {retrieveResult.Status}");
			}
			string description3 = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "WindowsHelloIsNotEnabledForCurrentUser");
			return Result.Error("Windows Hello", description3);
		}

		public void Logoff()
		{
			UpdateAuthenticationStatus(false);
		}

		private void UpdateAuthenticationStatus(bool isAuthenticated)
		{
			IsAuthenticated = isAuthenticated;
			_commonServices.MessageService.Send(this, "AuthenticationChanged", IsAuthenticated);
		}

		public async Task TrySetupWindowsHelloAsync(string userName)
		{
			if (await KeyCredentialManager.IsSupportedAsync())
			{
				string content = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Questions, "YourDeviceSupportsWindowsHelloAndYouCanUseItToAuthenticateWithTheApp");
				content += "\r\n\r\n";
				content += _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Questions, "DoYouWantToEnableWindowsHelloForYourNextSignInWithThisUser");
				string cancel = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.UI,"MaybeLater");
				if (await _commonServices.DialogService.ShowAsync("Windows Hello", content, "Ok", cancel))
				{
					await SetupWindowsHelloAsync(userName);
				}
				else
				{
					await TryDeleteCredentialAccountAsync(userName);
				}
			}
		}

		private async Task SetupWindowsHelloAsync(string userName)
		{
			var publicKey = await CreatePassportKeyCredentialAsync(userName);
			if (publicKey != null)
			{
				if (await RegisterPassportCredentialWithServerAsync(publicKey))
				{
					// When communicating with the server in the future, we pass a hash of the
					// public key in order to identify which key the server should use to verify the challenge.
					var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
					var publicKeyHash = hashProvider.HashData(publicKey);
					AppSettings.Current.WindowsHelloPublicKeyHint = CryptographicBuffer.EncodeToBase64String(publicKeyHash);
				}
			}
			else
			{
				await TryDeleteCredentialAccountAsync(userName);
			}
		}

		private async Task<IBuffer> CreatePassportKeyCredentialAsync(string userName)
		{
			// Create a new KeyCredential for the user on the device
			var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(userName, KeyCredentialCreationOption.ReplaceExisting);

			if (keyCreationResult.Status == KeyCredentialStatus.Success)
			{
				// User has autheniticated with Windows Hello and the key credential is created
				var credential = keyCreationResult.Credential;
				return credential.RetrievePublicKey();
			}
			else if (keyCreationResult.Status == KeyCredentialStatus.NotFound)
			{
				string content = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "ToProceedWindowsHelloNeedsToBeConfiguredInWindowsSettings");
				await _commonServices.DialogService.ShowAsync("Windows Hello", content);
			}
			else if (keyCreationResult.Status == KeyCredentialStatus.UnknownError)
			{
				string title = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "WindowsHelloError");
				string content = _commonServices.ResourceService.GetString<LoginService>(ResourceFiles.Errors, "TheKeyCredentialCouldNotBeCreatedPleaseTryAgain");
				await _commonServices.DialogService.ShowAsync(title, content);
			}

			return null;
		}

		const int NTE_NO_KEY = unchecked((int)0x8009000D);
		const int NTE_PERM = unchecked((int)0x80090010);

		static private async Task<bool> TryDeleteCredentialAccountAsync(string userName)
		{
			try
			{
				AppSettings.Current.WindowsHelloPublicKeyHint = null;
				await KeyCredentialManager.DeleteAsync(userName);
				return true;
			}
			catch (Exception ex)
			{
				switch (ex.HResult)
				{
					case NTE_NO_KEY:
						// Key is already deleted. Ignore this error.
						break;
					case NTE_PERM:
						// Access is denied. Ignore this error. We tried our best.
						break;
					default:
						System.Diagnostics.Debug.WriteLine(ex.Message);
						break;
				}
			}
			return false;
		}

		static private Task<bool> RegisterPassportCredentialWithServerAsync(IBuffer publicKey)
		{
			// TODO:
			// Register the public key and attestation of the key credential with the server
			// In a real-world scenario, this would likely also include:
			//      - Certificate chain for attestation endorsement if available
			//      - Status code of the Key Attestation result : Included / retrieved later / retry type
			return Task.FromResult(true);
		}
	}
}
