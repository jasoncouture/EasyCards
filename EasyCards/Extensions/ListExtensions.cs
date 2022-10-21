using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace EasyCards.Extensions;

public static class ListExtensions
{
    public static Il2CppReferenceArray<T> ToIl2CppReferenceArray<T>(this List<T> list) where T: Il2CppObjectBase
    {
        return new Il2CppReferenceArray<T>(list.ToArray());
    }
}