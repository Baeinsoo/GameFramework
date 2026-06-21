namespace GameFramework
{
    /// <summary> INetworkTime 파생 계산 확장 메서드. </summary>
    public static class NetworkTimeExtensions
    {
        /// <summary> 원격 보간 back-time = RTT/2. </summary>
        /// <remarks> 상한 clamp는 소비자 책임. </remarks>
        public static double RemoteBackTime(this INetworkTime networkTime)
            => networkTime.Rtt * 0.5;
    }
}
