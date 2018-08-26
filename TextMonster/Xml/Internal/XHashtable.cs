using System;
using System.Threading;

namespace TextMonster.Xml
{
  internal sealed class XHashtable<TValue>
  {
    XHashtableState state;

    public XHashtable(ExtractKeyDelegate extractKey, int capacity)
    {
      state = new XHashtableState(extractKey, capacity);
    }

    public bool TryGetValue(string key, int index, int count, out TValue value)
    {
      return state.TryGetValue(key, index, count, out value);
    }

    public TValue Add(TValue value)
    {
      TValue newValue;
      while (!state.TryAdd(value, out newValue))
      {
        var xhashtable = this;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(xhashtable, ref lockTaken);
          var xhashtableState = state.Resize();
          Thread.MemoryBarrier();
          state = xhashtableState;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(xhashtable);
        }
      }
      return newValue;
    }

    public delegate string ExtractKeyDelegate(TValue value);

    sealed class XHashtableState
    {
      readonly int[] buckets;
      readonly Entry[] entries;
      int numEntries;
      readonly ExtractKeyDelegate extractKey;

      public XHashtableState(ExtractKeyDelegate extractKey, int capacity)
      {
        buckets = new int[capacity];
        entries = new Entry[capacity];
        this.extractKey = extractKey;
      }

      public XHashtableState Resize()
      {
        if (numEntries < buckets.Length)
          return this;
        int num = 0;
        for (int index1 = 0; index1 < buckets.Length; ++index1)
        {
          int index2 = buckets[index1];
          if (index2 == 0)
            index2 = Interlocked.CompareExchange(ref buckets[index1], -1, 0);
          for (; index2 > 0; index2 = entries[index2].next != 0 ? entries[index2].next : Interlocked.CompareExchange(ref entries[index2].next, -1, 0))
          {
            if (extractKey(entries[index2].value) != null)
              ++num;
          }
        }
        int capacity;
        if (num < buckets.Length / 2)
        {
          capacity = buckets.Length;
        }
        else
        {
          capacity = buckets.Length * 2;
          if (capacity < 0)
            throw new OverflowException();
        }
        var xhashtableState = new XHashtableState(extractKey, capacity);
        for (int index1 = 0; index1 < buckets.Length; ++index1)
        {
          for (int index2 = buckets[index1]; index2 > 0; index2 = entries[index2].next)
          {
            TValue newValue;
            xhashtableState.TryAdd(entries[index2].value, out newValue);
          }
        }
        return xhashtableState;
      }

      public bool TryGetValue(string key, int index, int count, out TValue value)
      {
        int hashCode = ComputeHashCode(key, index, count);
        int entryIndex = 0;
        if (FindEntry(hashCode, key, index, count, ref entryIndex))
        {
          value = entries[entryIndex].value;
          return true;
        }
        value = default(TValue);
        return false;
      }

      public bool TryAdd(TValue value, out TValue newValue)
      {
        newValue = value;
        string key = extractKey(value);
        if (key == null)
          return true;
        int hashCode = ComputeHashCode(key, 0, key.Length);
        int index = Interlocked.Increment(ref numEntries);
        if (index < 0 || index >= buckets.Length)
          return false;
        entries[index].value = value;
        entries[index].hashCode = hashCode;
        Thread.MemoryBarrier();
        int entryIndex = 0;
        while (!FindEntry(hashCode, key, 0, key.Length, ref entryIndex))
        {
          entryIndex = entryIndex != 0 ? Interlocked.CompareExchange(ref entries[entryIndex].next, index, 0) : Interlocked.CompareExchange(ref buckets[hashCode & buckets.Length - 1], index, 0);
          if (entryIndex <= 0)
            return entryIndex == 0;
        }
        newValue = entries[entryIndex].value;
        return true;
      }

      bool FindEntry(int hashCode, string key, int index, int count, ref int entryIndex)
      {
        int index1 = entryIndex;
        int index2 = index1 != 0 ? index1 : buckets[hashCode & buckets.Length - 1];
        while (index2 > 0)
        {
          if (entries[index2].hashCode == hashCode)
          {
            string strB = extractKey(entries[index2].value);
            if (strB == null)
            {
              if (entries[index2].next > 0)
              {
                entries[index2].value = default(TValue);
                index2 = entries[index2].next;
                if (index1 == 0)
                {
                  buckets[hashCode & buckets.Length - 1] = index2;
                  continue;
                }
                entries[index1].next = index2;
                continue;
              }
            }
            else if (count == strB.Length && string.CompareOrdinal(key, index, strB, 0, count) == 0)
            {
              entryIndex = index2;
              return true;
            }
          }
          index1 = index2;
          index2 = entries[index2].next;
        }
        entryIndex = index1;
        return false;
      }

      static int ComputeHashCode(string key, int index, int count)
      {
        int num1 = 352654597;
        int num2 = index + count;
        for (int index1 = index; index1 < num2; ++index1)
          num1 += num1 << 7 ^ key[index1];
        int num3 = num1 - (num1 >> 17);
        int num4 = num3 - (num3 >> 11);
        return num4 - (num4 >> 5) & int.MaxValue;
      }

      struct Entry
      {
        public TValue value;
        public int hashCode;
        public int next;
      }
    }
  }
}
