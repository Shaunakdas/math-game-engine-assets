using UnityEngine;
using System.Collections;

//Why the heck NGUI didn't provide these functions? while it's crucial, and much faster than manually calling Add<T> + foreach loop
//However, The good news is, it's made the buffer public, anyway ...
public static class BetterListExtensions
{
    public static void AddRange<T>(this BetterList<T> list, T[] range)
    {
        if (range == null || range.Length == 0)
            return;
        list.AllocateMore(list.size + range.Length);
        int newSize = list.size + range.Length;
        for (int i = list.size; i < newSize; i++) {
            list.buffer[i] = range[i - list.size];
        }
        list.size += range.Length;
    }

    static void AllocateMore<T>(this BetterList<T> list, int minimumLength)
    {
        AllocateMore(list, minimumLength, false);
    }

    static void AllocateMore<T>(this BetterList<T> list, int minimumLength, bool forceAsItIs)
    {
        //	if(list.buffer != null && list.buffer.Length >= minimumLength)
        //		return;
        int newBuffer = forceAsItIs ? minimumLength : Mathf.ClosestPowerOfTwo(minimumLength);
        while (newBuffer < minimumLength) {
            newBuffer = newBuffer << 1;     
        }
        T[] newList = new T[newBuffer];
        if (list.buffer != null) {
            if (list.buffer.Length <= newBuffer)
                list.buffer.CopyTo(newList, 0);
            else
                System.Array.Copy(list.buffer, 0, newList, 0, newBuffer);
        }
        list.buffer = newList;
    
    }

    ///Force the Allocation to fulfill the required length;
    public static void ForceAllocation<T>(this BetterList<T> list, int length)
    {
        list.AllocateMore(length, true);
        list.size = length;
    }


    static public T GetOrAddTemporary<T>(Component t) where T : Component
    {
        var c = t.GetComponent<T>();
        if (!c)
        {
            c = t.gameObject.AddComponent<T>();
            c.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            }
        return c;
    }
}

