using GameFramework.World;
using NUnit.Framework;
using UnityEngine;

// World.Transform은 UnityEngine.Transform과 이름이 겹친다 — 이 파일 컨벤션대로
// 풀 네임스페이스로 한정한다(world-core-connection-architecture.md 참고).
public class TransformTeleportTests
{
    private static Entity MakeEntity()
    {
        var entity = new Entity("e1");
        entity.Add(new GameFramework.World.Transform());
        return entity;
    }

    [Test]
    public void 평범한_이동은_카운터를_올리지_않는다()
    {
        Entity entity = MakeEntity();

        entity.SetPosition(new Vector3(1f, 2f, 3f));

        Assert.AreEqual(0, entity.Get<GameFramework.World.Transform>().TeleportCount);
    }

    [Test]
    public void 텔레포트는_위치를_쓰고_카운터를_올린다()
    {
        Entity entity = MakeEntity();

        entity.Teleport(new Vector3(0f, 2200f, 0f));

        Assert.AreEqual(new Vector3(0f, 2200f, 0f), entity.GetPosition());
        Assert.AreEqual(1, entity.Get<GameFramework.World.Transform>().TeleportCount);
    }

    // 같은 자리로 텔레포트해도 "옮겼다"는 사실은 알려야 한다. SetPosition은 값이 같으면
    // 안 쓰고 빠져나가는데, 그 최적화에 카운터까지 딸려 가면 신호가 사라진다.
    [Test]
    public void 같은_자리로_텔레포트해도_카운터는_오른다()
    {
        Entity entity = MakeEntity();
        entity.SetPosition(new Vector3(5f, 0f, 0f));

        entity.Teleport(new Vector3(5f, 0f, 0f));

        Assert.AreEqual(1, entity.Get<GameFramework.World.Transform>().TeleportCount);
    }

    [Test]
    public void 여러_번_텔레포트하면_계속_오른다()
    {
        Entity entity = MakeEntity();

        entity.Teleport(new Vector3(1f, 0f, 0f));
        entity.Teleport(new Vector3(2f, 0f, 0f));
        entity.Teleport(new Vector3(3f, 0f, 0f));

        Assert.AreEqual(3, entity.Get<GameFramework.World.Transform>().TeleportCount);
    }
}
