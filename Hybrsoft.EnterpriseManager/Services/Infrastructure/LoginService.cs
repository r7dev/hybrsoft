using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using System;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class LoginService(IMessageService messageService, IDialogService dialogService, IUserService userService, IPasswordHasher passwordHasher) : ILoginService
	{
		public IMessageService MessageService { get; } = messageService;
		public IDialogService DialogService { get; } = dialogService;
		public IUserService UserService { get; } = userService;
		public IPasswordHasher PasswordHasher { get; } = passwordHasher;

		public bool IsAuthenticated { get; set; } = false;

		public async Task<bool> IsWindowsHelloEnabledAsync(string userName)
		{
			bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
			if (keyCredentialAvailable && !String.IsNullOrEmpty(userName))
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
			UserDto user = await UserService.GetUserByEmailAsync(userName, true);
			bool isUserAuthenticated = user != null && PasswordHasher.VerifyHashedPassword(user.Password, password);
			AppSettings.Current.UserID = isUserAuthenticated ? user.UserID : default;
			AppSettings.Current.UserName = isUserAuthenticated ? userName : default;
			UpdateAuthenticationStatus(isUserAuthenticated);
			return isUserAuthenticated
				? Result.Ok()
				: Result.Error("Login error", "Please, enter a valid username and password.");
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
					return Result.Error("Windows Hello", $"Cannot sign in with Windows Hello: {result.Status}");
				}
				return Result.Error("Windows Hello", $"Cannot sign in with Windows Hello: {retrieveResult.Status}");
			}
			return Result.Error("Windows Hello", "Windows Hello is not enabled for current user.");
		}

		public void Logoff()
		{
			UpdateAuthenticationStatus(false);
		}

		private void UpdateAuthenticationStatus(bool isAuthenticated)
		{
			IsAuthenticated = isAuthenticated;
			MessageService.Send(this, "AuthenticationChanged", IsAuthenticated);
		}

		public async Task TrySetupWindowsHelloAsync(string userName)
		{
			if (await KeyCredentialManager.IsSupportedAsync())
			{
				if (await DialogService.ShowAsync("Windows Hello", "Your device supports Windows Hello and you can use it to authenticate with the app.\r\n\r\nDo you want to enable Windows Hello for your next sign in with this user?", "Ok", "Maybe later"))
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
				await DialogService.ShowAsync("Windows Hello", "To proceed, Windows Hello needs to be configured in Windows Settings (Accounts -> Sign-in options)");
			}
			else if (keyCreationResult.Status == KeyCredentialStatus.UnknownError)
			{
				await DialogService.ShowAsync("Windows Hello Error", "The key credential could not be created. Please try again.");
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
