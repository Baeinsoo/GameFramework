using System.Threading.Tasks;

namespace GameFramework
{
    /// <summary>
    /// 맵 씬 로드/언로드 추상. 매치 환경 셋업의 일부로 host(Factory)가 오케스트레이션한다.
    /// 시뮬(World)은 엔진-프리라 씬 I/O를 다루지 않으므로 이 책임은 use-side(어댑터)에 둔다.
    /// </summary>
    public interface IMapLoader
    {
        Task LoadAsync(string mapId);
        Task UnloadAsync();
    }
}
