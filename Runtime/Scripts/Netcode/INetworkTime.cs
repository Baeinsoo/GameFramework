namespace GameFramework.Netcode
{
    /// <summary>
    /// 네트워크 동기 시간 소스. 양쪽(클·서) 공통 인터페이스.
    /// 시간동기 메커니즘(offset 추정·서버 피드백·dilation)은 구현체의 책임이며,
    /// 이 인터페이스는 그 출력(읽기 전용)만 노출한다.
    /// 불변식: PredictedTime = ServerNow + Rtt/2 (서버는 Rtt=0이라 둘이 일치).
    /// </summary>
    public interface INetworkTime
    {
        /// <summary> 현재 서버 시간(추정, 초). 서버=권위(자기 시간), 클라=predictedTime − RTT/2. </summary>
        double ServerNow { get; }

        /// <summary> client-ahead 예측 시간(초) — 내 입력이 서버에 닿을 시각. 서버에선 ServerNow와 일치. </summary>
        double PredictedTime { get; }

        /// <summary> 왕복지연(초). 서버=0. </summary>
        double Rtt { get; }
    }
}
