using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IDataContext
    {
        Type[] subscribedTypes { get; }

        void UpdateData<T>(T data);
        void Clear();
    }
}
