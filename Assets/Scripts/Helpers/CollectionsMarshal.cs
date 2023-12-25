using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

public static class CollectionsMarshal
{
	public static Span<T> AsSpan<T>(List<T> list)
    {
        var unsafeBox = UnsafeUtility.As<List<T>, StrongBox<T[]>>(ref list);
        return unsafeBox.Value.AsSpan(0, list.Count);
    }
}
