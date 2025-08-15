using System;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.Infrastructure.Commom
{
	public partial class RelayCommand(Action execute, Func<bool> canExecute) : ICommand
	{
		private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

		private readonly Func<bool> _canExecute = canExecute;

		public event EventHandler CanExecuteChanged;

		public RelayCommand(Action execute)
			: this(execute, null)
		{
		}

		public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

		public void Execute(object parameter) => _execute();

		public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public partial class RelayCommand<T>(Action<T> execute, Func<T, bool> canExecute) : ICommand
	{
		private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

		private readonly Func<T, bool> _canExecute = canExecute;

		public event EventHandler CanExecuteChanged;

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

		public void Execute(object parameter) => _execute((T)parameter);

		public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}
