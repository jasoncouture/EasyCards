using EasyCards.Services;
using StrongInject;

namespace EasyCards.Validation.Validators;

[Register(typeof(NoOpValidator), Scope.SingleInstance, typeof(IValidator))]
[Register(typeof(SpriteTextureValidator), Scope.SingleInstance, typeof(IValidator))]
public sealed class ValidatorsModule { }
