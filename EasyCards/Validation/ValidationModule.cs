using EasyCards.Validation.Validators;
using StrongInject;

namespace EasyCards.Validation;

[Register(typeof(ValidationAggregator), Scope.SingleInstance, typeof(IValidationAggregator))]
[RegisterModule(typeof(ValidatorsModule))]
public sealed class ValidationModule { }