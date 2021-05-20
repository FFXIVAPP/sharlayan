// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegatedCommand.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DelegatedCommand.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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