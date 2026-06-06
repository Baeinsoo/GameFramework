using System.Threading.Tasks;

namespace GameFramework
{
    public interface IGameFactory
    {
        Task<IGame> CreateAsync();
        Task DestroyAsync();
    }
}
