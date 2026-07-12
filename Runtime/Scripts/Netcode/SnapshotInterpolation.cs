using System.Collections.Generic;

namespace GameFramework.Netcode
{
    /// <summary>보간할 두 샘플의 인덱스와 그 사이 위치 <see cref="Alpha"/>[0,1]. 범위 밖이면 끝값 hold(Lower==Upper).</summary>
    public readonly struct BracketIndices
    {
        public readonly int Lower;
        public readonly int Upper;
        public readonly float Alpha;

        public BracketIndices(int lower, int upper, float alpha)
        {
            Lower = lower;
            Upper = upper;
            Alpha = alpha;
        }
    }

    /// <summary>
    /// 스냅샷 보간 — 오름차순 샘플 시각들 안에서 연속 renderTime을 감싸는 두 인덱스와 alpha를 찾는다.
    /// 절대 인덱스 조회가 아니라 브래킷 탐색이라 "그 틱이 버퍼에 없어서 스킵"이 구조적으로 불가.
    /// 범위 밖이면 끝값 hold, 빈 리스트면 {-1,-1}. 순수 — EditMode 테스트. (Fiedler snapshot interpolation.)
    /// </summary>
    public static class SnapshotInterpolation
    {
        public static BracketIndices Solve(IReadOnlyList<double> times, double renderTime)
        {
            int n = times.Count;
            if (n == 0)
            {
                return new BracketIndices(-1, -1, 0f);
            }
            if (n == 1 || renderTime <= times[0])
            {
                return new BracketIndices(0, 0, 0f);
            }
            if (renderTime >= times[n - 1])
            {
                return new BracketIndices(n - 1, n - 1, 0f);
            }

            // renderTime은 times[0]과 times[n-1] 사이 → 감싸는 쌍을 찾는다(위에서부터).
            for (int i = n - 2; i >= 0; i--)
            {
                if (times[i] <= renderTime)
                {
                    float alpha = (float)((renderTime - times[i]) / (times[i + 1] - times[i]));
                    return new BracketIndices(i, i + 1, alpha);
                }
            }
            return new BracketIndices(0, 0, 0f);
        }
    }
}
