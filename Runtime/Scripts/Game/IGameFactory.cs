using System.Threading.Tasks;

namespace GameFramework.Runner
{
    public interface IGameFactory
    {
        Task<IRunner> CreateAsync();
        Task DestroyAsync();
    }
}
