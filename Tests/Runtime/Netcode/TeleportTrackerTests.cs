using GameFramework.Netcode;
using NUnit.Framework;

public class TeleportTrackerTests
{
    // 처음 보는 엔티티는 텔레포트가 아니다 — 스폰이다. 여기서 true를 돌려주면 모든 스폰이
    // 텔레포트로 처리된다.
    [Test]
    public void 처음_본_엔티티는_텔레포트가_아니다()
    {
        var tracker = new TeleportTracker();

        Assert.IsFalse(tracker.Observe("e1", 0));
        Assert.IsFalse(tracker.Observe("e2", 7));
    }

    [Test]
    public void 값이_그대로면_텔레포트가_아니다()
    {
        var tracker = new TeleportTracker();
        tracker.Observe("e1", 3);

        Assert.IsFalse(tracker.Observe("e1", 3));
        Assert.IsFalse(tracker.Observe("e1", 3));
    }

    [Test]
    public void 값이_바뀌면_한_번만_텔레포트다()
    {
        var tracker = new TeleportTracker();
        tracker.Observe("e1", 3);

        Assert.IsTrue(tracker.Observe("e1", 4));
        Assert.IsFalse(tracker.Observe("e1", 4));
    }

    // 스냅샷이 유실돼 여러 번 오른 값이 한꺼번에 와도 알아채야 한다.
    [Test]
    public void 한꺼번에_여러_칸_올라도_알아챈다()
    {
        var tracker = new TeleportTracker();
        tracker.Observe("e1", 3);

        Assert.IsTrue(tracker.Observe("e1", 9));
    }

    [Test]
    public void 엔티티끼리_섞이지_않는다()
    {
        var tracker = new TeleportTracker();
        tracker.Observe("e1", 1);
        tracker.Observe("e2", 1);

        Assert.IsTrue(tracker.Observe("e1", 2));
        Assert.IsFalse(tracker.Observe("e2", 1));
    }

    [Test]
    public void 잊은_엔티티는_다시_처음_본_것이_된다()
    {
        var tracker = new TeleportTracker();
        tracker.Observe("e1", 1);
        tracker.Forget("e1");

        Assert.IsFalse(tracker.Observe("e1", 5));
    }
}
