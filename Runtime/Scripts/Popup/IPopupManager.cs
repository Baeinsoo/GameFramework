using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IPopupManager
    {
        T GetPopup<T>(Transform parent = null) where T : IPopup;
        void CloseAll();
    }
}
