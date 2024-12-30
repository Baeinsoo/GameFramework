using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoGamePresenter<T> : MonoBehaviour, IGamePresenter<T> where T : IGame
    {
        public T game { get; protected set; }
    }
}
