using System;
using System.Collections.Generic;

namespace GameFramework
{
    /// <summary>
    /// 스코프가 살아있는 동안 구독을 유지하고, 스코프 종료 시 한꺼번에 해제하는 DI 엔트리포인트 base.
    /// VContainer가 Initialize(구독)/Dispose(해제)를 구동한다.
    /// </summary>
    public abstract class MessageHandlerBase : VContainer.Unity.IInitializable, IDisposable
    {
        private readonly List<IDisposable> subscriptions = new();

        void VContainer.Unity.IInitializable.Initialize() => Subscribe();

        /// <summary>
        /// Track된 모든 구독을 해제한다. 구독 외 추가 teardown이 필요한 핸들러(예: 서버
        /// GameInfoMessageHandler의 runner 리스너 해제)는 이 메서드를 override하고 base.Dispose()를 먼저 호출한다.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var s in subscriptions) s.Dispose();
            subscriptions.Clear();
        }

        /// <summary>구독을 걸고 그 결과를 <see cref="Track"/>로 등록한다.</summary>
        protected abstract void Subscribe();

        /// <summary>구독 핸들(IDisposable)을 스코프 수명에 묶는다. 스코프 종료 시 자동 해제.</summary>
        protected void Track(IDisposable subscription) => subscriptions.Add(subscription);
    }
}
