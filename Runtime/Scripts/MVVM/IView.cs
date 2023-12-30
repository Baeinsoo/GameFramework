using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    namespace MVVM
    {
        public interface IView
        {
            void BindViewModel<T>(T viewModel) where T : IViewModel;
        }
    }
}
