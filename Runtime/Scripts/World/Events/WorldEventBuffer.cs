using System;
using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// 단일 폴리모픽 이벤트 큐. Generation/와이어 어댑터가 Append, Application과 Bridge가
    /// 같은 Snapshot을 본 뒤 Clear. 비동기 네트워크 수신과 결정론적 틱 드레인 사이의 정렬 버퍼.
    /// 슬라이스 3은 commit gate 없이 매 드레인 = commit. Stage ④에서 tick 분리·dedupe 추가.
    /// </summary>
    public class WorldEventBuffer
    {
        private readonly List<WorldEvent> _events = new List<WorldEvent>();

        /// <summary>이벤트 1개 append. null은 ArgumentNullException.</summary>
        public void Append(WorldEvent e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            _events.Add(e);
        }

        /// <summary>현재 누적된 이벤트의 읽기 전용 뷰. 자체 mutation 없음.</summary>
        public IReadOnlyList<WorldEvent> Snapshot => _events;

        /// <summary>누적된 이벤트 전부 제거. 드레인 후 호출.</summary>
        public void Clear() => _events.Clear();

        /// <summary>현재 누적 개수.</summary>
        public int Count => _events.Count;
    }
}
