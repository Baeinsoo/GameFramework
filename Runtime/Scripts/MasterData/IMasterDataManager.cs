using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public interface IMasterDataManager
    {
        Task LoadMasterData();

        void RegisterMasterData<T>(IEnumerable<T> collection) where T : IMasterData;

        T GetMasterData<T>(string code) where T : IMasterData;
        IEnumerable<T> GetMasterData<T>() where T : IMasterData;

        bool TryGetMasterData<T>(string code, out T masterData) where T : IMasterData;
    }
}
