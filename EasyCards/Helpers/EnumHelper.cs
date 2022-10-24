using System;
using System.Linq;

namespace EasyCards.Helpers;

public static class EnumHelper
{
    public static bool IsValidIdentifierForEnum<T>(string identifier) where T: Enum
    {
        return System.Enum.GetNames(typeof(T)).ToList().Contains(identifier);
    }
}