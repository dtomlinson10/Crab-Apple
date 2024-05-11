//Using this for the ICommand interface, creates commands that bind to UI elements eg buttons.
//RelayCommand.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrapApple
{
    public class RelayCommand : ICommand //used for icommand, follows conventional WPF model MVVM and just makes life easier (XAML data binding<-----)
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            //check if the command can be executed based on the provided predicate
            return _canExecute == null || _canExecute(parameter);
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            //execute the command with the provided parameter
            _execute(parameter);
        }
    }
}
