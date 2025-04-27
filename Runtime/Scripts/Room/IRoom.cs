using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface IRoom : IInitializableAsync, IDeinitializableAsync
    {
        IGame game { get; }

        Task StartGameAsync();

        void OnPlayerConnect(IConnectionData data);
        void OnPlayerDisconnect(IConnectionData data);
    }
}
