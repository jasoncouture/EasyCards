using System;
using System.Linq;

namespace EasyCards.Extensions;

public static class EnumExtensions
{
    public static bool IsValidEnumValue<T>(this T enumToCheck, string enumValueToCheck) where T : Enum
    {
        return Enum.GetNames(typeof(T)).ToList().Contains(enumValueToCheck);
    }
}
