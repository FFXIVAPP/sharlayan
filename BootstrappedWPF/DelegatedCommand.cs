namespace BootstrappedWPF {
    using System;
    using System.Windows.Input;

    public class DelegatedCommand : ICommand {
        private readonly Func<object, bool> _canExecute;

        private readonly Action<object> _execute;

        public DelegatedCommand(Action<object> execute) : this(execute, null) { }

        public DelegatedCommand(Action<object> execute, Func<object, bool>? canExecute) {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? (x => true);
        }

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) {
            return this._canExecute(parameter);
        }

        public void Execute(object parameter) {
            this._execute(parameter);
        }

        public void Refresh() {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}