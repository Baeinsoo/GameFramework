namespace GameFramework.World
{
    /// <summary>
    /// 모든 월드 이벤트의 폴리모픽 베이스. 불변 데이터 레코드.
    /// Generation이 만들고 Application이 상태에 쓰며 Bridge가 프레젠테이션으로 fan-out한다.
    /// Stage ④에서 tick 스탬프 + source 태그(Predicted/Confirmed)가 여기에 추가될 예정.
    /// </summary>
    public abstract record WorldEvent;
}
