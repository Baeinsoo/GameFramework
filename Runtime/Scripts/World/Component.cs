namespace GameFramework.World
{
    /// <summary>
    /// CBD 컴포넌트의 추상 베이스. 데이터와 파생 속성만 가지며(Anemic), 상태 변경 로직은
    /// System에 둔다. <see cref="Owner"/>는 <see cref="Entity.Add{T}"/> 시 자동 설정된다.
    /// </summary>
    public abstract class Component
    {
        /// <summary>이 컴포넌트를 소유한 Entity. Entity에 부착/해제될 때 설정/해제된다.</summary>
        public Entity Owner { get; internal set; }
    }
}
