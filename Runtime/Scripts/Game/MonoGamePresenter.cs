using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoGamePresenter : MonoBehaviour, IGamePresenter
    {
        public IGame game { get; protected set; }
    }
}
