using System.Collections.Generic;

namespace GameFramework.Runner
{
    /// <summary>
    /// 언제 출발할지를 정한다. 전원이 준비하거나 대기 상한이 지나면 출발틱을 확정한다 —
    /// 지금 틱에서 leadTicks 뒤다. 언리얼 AGameMode의 ReadyToStartMatch + bDelayedStart에 해당.
    /// <para>
    /// 그 여유를 화면에서 어떻게 쓸지(몇 초는 "대기 중", 몇 초는 3·2·1)는 게이트가 정하지 않는다 —
    /// 보여주는 쪽의 몫이다.
    /// </para>
    /// <para>
    /// 시간이 아니라 틱으로 세는 이유: 출발틱이 곧 결과물이고, 클·서가 같은 숫자를 봐야 한다.
    /// </para>
    /// </summary>
    public sealed class MatchStartGate
    {
        private readonly HashSet<string> _ready = new HashSet<string>();
        private readonly long _waitCapTicks;
        private readonly long _leadTicks;

        //  대기 상한을 재기 시작한 틱. 첫 Tick에서 정해진다 — 생성자(DI 조립) 시점부터 재면
        //  맵 로딩 시간이 상한을 갉아먹는다.
        private long _armTick;
        private bool _armed;

        public MatchStartGate(int expectedPlayers, long waitCapTicks, long leadTicks)
        {
            ExpectedPlayers = expectedPlayers;
            _waitCapTicks = waitCapTicks;
            _leadTicks = leadTicks;
        }

        public MatchPhase Phase { get; private set; } = MatchPhase.WaitingForPlayers;
        public int ExpectedPlayers { get; }
        public int ReadyCount => _ready.Count;

        /// <summary>게임플레이가 시작될 틱. 아직 확정 전이면 long.MaxValue.</summary>
        public long StartTick { get; private set; } = long.MaxValue;

        /// <summary>같은 사람이 여러 번 보내도 한 번으로 센다. 이미 출발했으면 무시.</summary>
        public void MarkReady(string userId)
        {
            if (Phase != MatchPhase.WaitingForPlayers || string.IsNullOrEmpty(userId))
            {
                return;
            }

            _ready.Add(userId);
        }

        /// <summary>
        /// 페이즈 전이는 여기서만 일어난다. 메시지가 도착한 순간에 바꾸면 틱 중간에 페이즈가 갈려,
        /// 같은 틱을 보는 시스템들이 서로 다른 답을 본다.
        /// </summary>
        public void Tick(long tick)
        {
            if (_armed == false)
            {
                _armTick = tick;
                _armed = true;
            }

            switch (Phase)
            {
                case MatchPhase.WaitingForPlayers:
                    if (_ready.Count >= ExpectedPlayers || tick - _armTick >= _waitCapTicks)
                    {
                        StartTick = tick + _leadTicks;
                        Phase = MatchPhase.Countdown;
                    }
                    break;

                case MatchPhase.Countdown:
                    //  한 번 확정된 출발틱은 무슨 일이 있어도 밀지 않는다 —
                    //  이미 3-2-1을 보고 있는 사람이 배신당한다.
                    if (tick >= StartTick)
                    {
                        Phase = MatchPhase.InProgress;
                    }
                    break;
            }
        }

        /// <summary>매치가 끝났음을 알린다. 이후 Tick은 아무것도 바꾸지 않는다.</summary>
        public void Finish() => Phase = MatchPhase.Finished;
    }
}
