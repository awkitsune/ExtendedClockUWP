using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedClock.Model.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute)
            : this(execute, null)
        { }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute is null.");

            this.execute = execute;
            this.canExecute = canExecute;
            this.RaiseCanExecuteChangedAction = RaiseCanExecuteChanged;
            SimpleCommandManager.AddRaiseCanExecuteChangedAction(ref RaiseCanExecuteChangedAction);
        }

        ~DelegateCommand()
        {
            RemoveCommand();
        }

        public void RemoveCommand()
        {
            SimpleCommandManager.RemoveRaiseCanExecuteChangedAction(RaiseCanExecuteChangedAction);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute;
        }

        public void Execute(object parameter)
        {
            execute();
            SimpleCommandManager.RefreshCommandStates();
        }

        public bool CanExecute
        {
            get { return canExecute == null || canExecute(); }
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private readonly Action RaiseCanExecuteChangedAction;

        public event EventHandler CanExecuteChanged;

    }
}
