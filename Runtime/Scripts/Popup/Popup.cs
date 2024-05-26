using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class Popup : MonoBehaviour, IPopup
    {
        [field: SerializeField]
        public bool autoClose { get; private set; } = true;

        public event Action onClose;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);

            onClose?.Invoke();
        }
    }
}
