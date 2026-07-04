using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>예측 위치와 서버 권위 위치의 차이가 임계값을 넘으면 롤백해야 하는지 판정한다(순수).</summary>
    public static class ReconcileGate
    {
        public static bool ShouldReconcile(Vector3 predicted, Vector3 authoritative, float threshold)
        {
            return Vector3.Distance(predicted, authoritative) > threshold;
        }
    }
}
