using MessagePipe;
using VContainer;

namespace GameFramework
{
    /// <summary>
    /// MessagePipe의 <c>RegisterMessageBroker</c> 대신 쓰는 등록. 발행·구독 인터페이스는 그대로라
    /// 호출부는 바뀌지 않고, 핸들러를 부르는 순서만 구독 순서로 고정된다(이유는 브로커 주석 참고).
    ///
    /// 쓰지 않는 변형(Async/Buffered)은 등록하지 않는다 — 필요해지면 그때 브로커를 만들어 붙인다.
    /// </summary>
    public static class OrderedMessageBrokerRegistration
    {
        public static IContainerBuilder RegisterOrderedMessageBroker<TMessage>(this IContainerBuilder builder)
        {
            //  팩토리로 등록한다. Register<T>(Lifetime)은 VContainer가 리플렉션으로 생성자를
            //  찾는데, IL2CPP 빌드에서는 아무도 호출하지 않는 생성자가 스트리핑에 잘려 나가
            //  "injectable constructor를 못 찾는다"로 죽는다(iOS에서 실측). 여기서 new를
            //  직접 쓰면 린커가 그 호출을 보므로 잘리지 않고, 리플렉션도 타지 않는다.
            builder.Register(_ => new OrderedMessageBroker<TMessage>(), Lifetime.Singleton)
                .As<IPublisher<TMessage>>()
                .As<ISubscriber<TMessage>>();

            return builder;
        }

        public static IContainerBuilder RegisterOrderedMessageBroker<TKey, TMessage>(this IContainerBuilder builder)
        {
            //  위와 같은 이유로 팩토리 등록.
            builder.Register(_ => new OrderedKeyedMessageBroker<TKey, TMessage>(), Lifetime.Singleton)
                .As<IPublisher<TKey, TMessage>>()
                .As<ISubscriber<TKey, TMessage>>();

            return builder;
        }
    }
}
