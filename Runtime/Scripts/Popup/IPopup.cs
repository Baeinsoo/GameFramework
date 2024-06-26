using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IPopup
    {
        bool autoClose { get; }

        event Action onClose;

        void Show();
        void Close();
    }
}
