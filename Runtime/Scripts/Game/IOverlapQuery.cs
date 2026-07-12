namespace GameFramework
{
    /// <summary>
    /// 범위(구) 안에 겹치는 엔티티들의 id를 반환하는 오버랩 쿼리 포트.
    /// <see cref="ICollisionQuery"/>(캡슐 sweep)의 짝. 엔진 물리(Physics.OverlapSphere)에
    /// 직결되지 않도록 주입한다. 구체는 사이드별(LOPOverlapQuery)로, collider→엔티티 매핑을 담당한다.
    /// </summary>
    public interface IOverlapQuery
    {
        /// <summary>중심 <paramref name="center"/>·반지름 <paramref name="radius"/> 구 안에
        /// 겹치는 엔티티들의 id(World.Entity.Id)를 반환한다. 겹치는 게 없으면 빈 배열.</summary>
        string[] OverlapSphere(System.Numerics.Vector3 center, float radius);
    }
}
