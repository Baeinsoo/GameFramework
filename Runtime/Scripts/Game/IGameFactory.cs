using System.Threading.Tasks;

namespace GameFramework
{
    public interface IGameFactory
    {
        Task<IRunner> CreateAsync();
        Task DestroyAsync();
    }
}
