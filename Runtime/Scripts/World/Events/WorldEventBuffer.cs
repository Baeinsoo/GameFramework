using System;
using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// 단일 폴리모픽 이벤트 큐. Generation/와이어 어댑터가 Append, egress sink가 Snapshot을 본 뒤 Clear.
    /// 재생(확정 전 재시뮬) 구간은 Suppress() 스코프로 감싸 그 사이 Append를 버린다 — 이미 라이브에
    /// 방출된 연출이 재생 때 재발화하지 않도록(commit gate).
    /// </summary>
    public class WorldEventBuffer
    {
        private readonly List<WorldEvent> _events = new List<WorldEvent>();
        private int _suppressDepth;

        /// <summary>이벤트 1개 append. 억제 스코프 안이면 무시된다. null은 ArgumentNullException.</summary>
        public void Append(WorldEvent e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (_suppressDepth > 0) return;
            _events.Add(e);
        }

        /// <summary>이 스코프가 살아있는 동안 Append를 버린다. Dispose 시 해제(중첩 안전).</summary>
        public IDisposable Suppress() => new SuppressScope(this);

        /// <summary>현재 누적된 이벤트의 읽기 전용 뷰. 자체 mutation 없음.</summary>
        public IReadOnlyList<WorldEvent> Snapshot => _events;

        /// <summary>누적된 이벤트 전부 제거. 드레인 후 호출.</summary>
        public void Clear() => _events.Clear();

        /// <summary>현재 누적 개수.</summary>
        public int Count => _events.Count;

        private sealed class SuppressScope : IDisposable
        {
            private WorldEventBuffer _buffer;

            public SuppressScope(WorldEventBuffer buffer)
            {
                _buffer = buffer;
                _buffer._suppressDepth++;
            }

            public void Dispose()
            {
                if (_buffer == null) return;   // 이중 Dispose 무시
                _buffer._suppressDepth--;
                _buffer = null;
            }
        }
    }
}
