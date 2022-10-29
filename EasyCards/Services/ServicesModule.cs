using StrongInject;

namespace EasyCards.Services;

[Register(typeof(JsonDeserializer), Scope.SingleInstance, typeof(IJsonDeserializer))]
[Register(typeof(SpriteLoader), Scope.SingleInstance, typeof(ISpriteLoader))]
[Register(typeof(CardRepository), Scope.SingleInstance, typeof(ICardRepository))]
public sealed class ServicesModule { }
