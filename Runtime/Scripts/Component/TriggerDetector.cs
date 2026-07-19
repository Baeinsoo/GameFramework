using UnityEngine;
using System;

namespace GameFramework
{
    public class TriggerDetector : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnter;
        public event Action<Collider> onTriggerStay;
        public event Action<Collider> onTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
    }
}
