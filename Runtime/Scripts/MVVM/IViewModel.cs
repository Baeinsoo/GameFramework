using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace GameFramework
{
    namespace MVVM
    {
        public interface IViewModel : INotifyPropertyChanged
        {
            void BindModel<T>(T model) where T : IModel;
        }
    }
}
