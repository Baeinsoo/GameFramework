namespace GameFramework.World
{
    /// <summary>물리 엔진이 이 몸을 어떻게 취급할지. Box2D b2BodyType / Unity RigidbodyType2D와 같은 세 값.</summary>
    public enum BodyKind
    {
        Static,
        Kinematic,
        Dynamic,
    }

    /// <summary>
    /// 이 엔티티를 물리 엔진에 어떻게 세울지. 값은 게임이 정한다(엔티티를 만드는 쪽이 붙인다) —
    /// 몸을 만드는 팩토리가 여기서 기본값을 지어내면 시뮬이 쓰는 몸과 어긋난다.
    /// 모양은 별개다(<see cref="CapsuleShape"/>/<see cref="DiscShape"/>) — 그쪽은 엔진을 모르는
    /// 코어 sweep도 같이 읽는 순수 기하다.
    /// </summary>
    public class PhysicsConfig : Component
    {
        public BodyKind Kind { get; }

        /// <summary>회전을 잠글지. 캐릭터는 넘어지면 안 되고, 동전은 굴러야 한다.</summary>
        public bool FreezeRotation { get; }

        /// <summary>막지 않고 통과시키며 접촉만 알리는 몸인지(Box2D의 sensor).</summary>
        public bool IsTrigger { get; }

        /// <summary>물리 엔진이 이 몸의 모션을 소유하는가. 참이면 우리가 밀어넣지 않고 읽어온다.</summary>
        public bool PhysicsOwnsMotion => Kind == BodyKind.Dynamic;

        public PhysicsConfig(BodyKind kind, bool freezeRotation, bool isTrigger)
        {
            Kind = kind;
            FreezeRotation = freezeRotation;
            IsTrigger = isTrigger;
        }
    }
}
