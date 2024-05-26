using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    [DontDestroyMonoSingleton]
    public class PopupManager : MonoSingleton<PopupManager>, IPopupManager
    {
        public Canvas popupCanvas;

        private HashSet<IPopup> popups;

        protected override void Awake()
        {
            base.Awake();

            popups = new HashSet<IPopup>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CloseAll();

            popups = null;
        }

        public T GetPopup<T>(Transform parent = null) where T : IPopup
        {
            var popupPrefab = FindPopupPrefab<T>();
            if (popupPrefab == null)
            {
                throw new Exception($"There is no corresponding PopupPrefab. Type: {typeof(T).Name}");
            }

            var clone = Instantiate(popupPrefab, parent ?? popupCanvas.transform);
            clone.SetActive(false);

            var popup = clone.GetComponent<T>();
            popup.onClose += () =>
            {
                OnPopupClose(popup);
            };

            popups.Add(popup);

            return popup;
        }

        private GameObject FindPopupPrefab<T>() where T : IPopup
        {
            var candidate = PopupReferences.instance.popupPrefabs?.FirstOrDefault(x => x.GetType() == typeof(T));
            if (candidate == null)
            {
                Debug.LogWarning($"candidate is null.");
                return default;
            }

            return candidate.gameObject;
        }

        public void CloseAll()
        {
            foreach (var popup in popups)
            {
                if (popup is Popup p && p != null)
                {
                    Destroy(p.gameObject);
                }
            }

            popups.Clear();
        }

        private void OnPopupClose(IPopup popup)
        {
            if (popup is Popup p && p != null)
            {
                Destroy(p.gameObject);
            }

            popups.Remove(popup);
        }
    }
}
