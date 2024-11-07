using System.ComponentModel;

namespace Hybrsoft.EnterpriseManager.Tools.DependencyExpressions
{
	public interface INotifyExpressionChanged : INotifyPropertyChanged
	{
		void NotifyPropertyChanged(string propertyName);
	}
}
