using System.Threading.Tasks;

namespace GameFramework
{
    public interface IDeinitializableAsync
    {
        Task DeinitializeAsync();
    }
}
