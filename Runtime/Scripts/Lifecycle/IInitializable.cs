using System.Threading.Tasks;

namespace GameFramework
{
    public interface IInitializableBase
    {
        bool initialized { get; }
    }

    public interface IInitializableAsync : IInitializableBase
    {
        Task InitializeAsync();
    }
}
