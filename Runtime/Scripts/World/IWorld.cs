namespace GameFramework.World
{
    public interface IWorld
    {
        EntityRegistry EntityRegistry { get; }
        WorldEventBuffer EventBuffer { get; }
        void Tick(long tick, float deltaTime);

        /// <summary>
        /// 이번 틱 시뮬 상태를 보관한다. 되돌릴 수 있는 건 여기 담긴 것뿐이다.
        /// 무엇을 담을지는 각 게임의 월드가 정한다 — 부르는 쪽(넷코드)은 내용을 모른다.
        /// GGPO <c>save_game_state</c> 대응.
        /// </summary>
        void SaveState(long tick);

        /// <summary>
        /// 그 틱 상태로 되돌린다. 기록이 없으면 아무것도 바꾸지 않고 false.
        /// GGPO <c>load_game_state</c> 대응.
        /// </summary>
        bool LoadState(long tick);

        /// <summary>
        /// 보관을 시작한 가장 이른 틱. 조회 실패가 "아직 살지 않은 틱"인지 "밀려난 틱"인지 가른다 —
        /// 앞은 손대면 안 되고 뒤는 따라잡아야 한다.
        /// </summary>
        long? FirstSavedTick { get; }

        /// <summary>가장 최근 보관 틱(진단용).</summary>
        long? LatestSavedTick { get; }

        /// <summary>
        /// 보관된 위치·속도를 읽는다. 예측이 서버와 얼마나 어긋났는지 재는 데 쓴다 —
        /// 위치는 게임 종류와 무관한 값이라 이걸 노출해도 부르는 쪽이 게임을 알게 되지 않는다.
        /// </summary>
        bool TryGetSavedMotion(long tick, string entityId, out Netcode.EntitySnapshot motion);
    }
}
