
namespace GameFramework
{
    public interface IDataStoreManager
    {
        void Register<T>(T dataStore) where T : IDataStore;
        T Get<T>() where T : IDataStore;
        void UpdateData<T>(T data);
    }
}
