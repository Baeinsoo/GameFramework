using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    [CreateAssetMenu(fileName = "PopupReferences", menuName = "GameFramework/PopupReferences")]
    public class PopupReferences : ScriptableObjectSingleton<PopupReferences>
    {
        public Popup[] popupPrefabs;
    }
}
