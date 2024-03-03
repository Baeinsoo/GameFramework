using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IDataStorage
    {
        void Set<T>(T value);
        void Set<T>(string key, T value);
        bool Get<T>(out T value, bool delete = false);
        bool Get<T>(string key, out T value, bool delete = false);
    }
}
