using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Http;

namespace GameFramework.Tests.Http
{
    /// <summary>토큰을 정해줄 수 있는 가짜 공급자. forceRefresh 값을 호출 순서대로 기록한다.</summary>
    public sealed class FakeAccessTokenProvider : IAccessTokenProvider
    {
        private readonly Func<bool, string> resolve;

        public List<bool> Calls { get; } = new List<bool>();

        public FakeAccessTokenProvider(Func<bool, string> resolve)
        {
            this.resolve = resolve;
        }

        public static FakeAccessTokenProvider Returning(string accessToken)
        {
            return new FakeAccessTokenProvider(_ => accessToken);
        }

        public UniTask<string> GetAccessTokenAsync(bool forceRefresh, CancellationToken cancellationToken)
        {
            Calls.Add(forceRefresh);
            return UniTask.FromResult(resolve(forceRefresh));
        }
    }
}
