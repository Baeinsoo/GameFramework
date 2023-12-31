using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    namespace MVVM
    {
        public class RelayCommand : ICommand
        {
            private readonly Action execute;
            private readonly Func<bool> canExecute;

            public event EventHandler CanExecuteChanged;

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
                this.canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return canExecute == null || canExecute();
            }

            public void Execute(object parameter)
            {
                execute();
            }

            public void OnCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
