using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface IRoom : IInitializableAsync, IDeinitializableAsync
    {
        IGame game { get; }

        Task StartGameAsync();
    }

    public interface IServerRoom : IRoom
    {
        void OnPlayerConnect(IConnectionData data);
        void OnPlayerDisconnect(IConnectionData data);
    }
}
