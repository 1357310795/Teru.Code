using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Teru.Code.Wpf.Mvvm
{
    public abstract class AsyncCommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract Task ExecuteAsync();

        public abstract bool CanExecuteTask();

        [DebuggerStepThrough]
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        protected bool CanExecute()
        {
            return CanExecuteTask();
        }

        protected async void Execute()
        {
            if (CanExecuteTask() == false)
            {
                return;
            }

            RaiseCanExecuteChanged();

            try
            {
                await ExecuteAsync();
            }
            finally
            {
                RaiseCanExecuteChanged();
            }
        }
    }

    public abstract class AsyncCommandBase<T> : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public abstract Task ExecuteAsync(T parameter);

        public abstract bool CanExecuteTask(T parameter);

        bool ICommand.CanExecute(object parameter)
        {
            if (typeof(T).IsValueType && parameter == null)
            {
                return false;
            }

            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            if (typeof(T).IsValueType && parameter == null)
            {
                return;
            }

            Execute((T)parameter);
        }

        protected bool CanExecute(T parameter)
        {
            return CanExecuteTask(parameter);
        }

        protected async void Execute(T parameter)
        {
            if (CanExecuteTask(parameter) == false)
            {
                return;
            }

            RaiseCanExecuteChanged();

            try
            {
                await ExecuteAsync(parameter);
            }
            finally
            {
                RaiseCanExecuteChanged();
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}