using UnityEngine;

namespace GameFramework
{
    public interface IDataContextManager
    {
        void Register<T>(T dataContext) where T : IDataContext;
        T Get<T>() where T : IDataContext;
        void UpdateData<T>(T data);
    }
}
