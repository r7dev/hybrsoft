namespace Hybrsoft.UI.Windows.Services
{
	public interface IWindowsSecurityService
	{
		string EncryptData(object value);
		TResult DecryptData<TResult>(string encryptedData);
	}
}
