using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class Restorer : IDisposable
    {
        public event Action action;

        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            action?.Invoke();
            action = null;

            disposed = true;
        }
    }
}
