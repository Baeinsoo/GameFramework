using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramework
{
    namespace MVVM
    {
        public interface ICommand
        {
            event EventHandler CanExecuteChanged;

            bool CanExecute(object parameter);
            void Execute(object parameter);
        }
    }
}
