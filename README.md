# GameFramework

앱 비종속 엔진 인프라 패키지(`com.baegames.gameframework`). 결정론 시뮬 추상, World Core,
wire 추상, FSM, 메시지 버스 등 다음 프로젝트에서도 그대로 재사용할 것만 둔다.

## 사용 측 계약 (use-side)

`package.json`의 `dependencies`로 표현하지 않고, **쓰는 쪽 프로젝트가 직접 보유**하는 패키지들이다.
빠지면 컴파일이 깨진다.

| 패키지 | 쓰는 곳 |
|---|---|
| `jp.hadashikick.vcontainer` | DI — 엔트리포인트, 등록 확장 |
| `com.cysharp.unitask` | 비동기 |
| `com.cysharp.messagepipe` | 메시지 버스 — `MessageHandlerBase`, `OrderedMessageBroker` |
| `com.cysharp.messagepipe.vcontainer` | 위 버스의 DI 등록(`RegisterMessagePipe`) |

## 메시지 버스 — `RegisterOrderedMessageBroker`를 쓸 것

MessagePipe 기본 브로커(`RegisterMessageBroker`)는 **핸들러 호출 순서를 보장하지 않는다** —
구독 해제된 자리를 재사용하기 때문에, 구독·해제를 반복하면 나중에 구독한 쪽이 먼저 불린다.
한 메시지를 여러 구독자가 나눠 받는 배선에서는 그 순서가 곧 동작이므로,
`OrderedMessageBroker`(구독 순서 보장)를 등록하는 `RegisterOrderedMessageBroker<T>`를 쓴다.
발행·구독 인터페이스(`IPublisher`/`ISubscriber`)는 MessagePipe 것 그대로라 호출부는 같다.
