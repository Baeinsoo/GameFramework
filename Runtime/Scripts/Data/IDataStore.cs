using System;

namespace GameFramework
{
    public interface IDataStore
    {
        Type[] subscribedTypes { get; }

        void UpdateData<T>(T data);
        void Clear();
    }
}
