
namespace GameFramework
{
    public interface IDataUpdater
    {
        void AddListener(object listener);
        void RemoveListener(object listener);
    }
}
