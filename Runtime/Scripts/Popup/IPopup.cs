using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IPopup
    {
        event Action onClose;

        void Show();
        void Close();
    }
}
