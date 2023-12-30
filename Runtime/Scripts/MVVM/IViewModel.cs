using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    namespace MVVM
    {
        public interface IViewModel
        {
            void BindModel<T>(T model) where T : IModel;
        }
    }
}
