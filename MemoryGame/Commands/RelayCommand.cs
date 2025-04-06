using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemoryGame.Commands
{
    // Generic RelayCommand that accepts a parameter
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter == null && typeof(T).IsValueType)
                return _canExecute(default);

            if (parameter == null || parameter is T)
                return _canExecute((T)parameter);

            return false;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                if (typeof(T).IsValueType)
                    _execute(default);
                else
                    _execute((T)parameter);
                return;
            }

            if (parameter is T tParam)
            {
                _execute(tParam);
                return;
            }

            // Try to convert the parameter
            try
            {
                var convertedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                _execute(convertedParameter);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Parameter type mismatch. Expected {typeof(T).Name}.", nameof(parameter));
            }
        }
    }

    // Non-generic RelayCommand that doesn't require a parameter
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
} 