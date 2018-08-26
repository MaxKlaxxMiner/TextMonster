using System;
using System.Threading;

namespace TextMonster.Xml
{
  internal sealed class XHashtable<TValue>
  {
    private XHashtable<TValue>.XHashtableState state;
    private const int StartingHash = 352654597;

    public XHashtable(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
    {
      this.state = new XHashtable<TValue>.XHashtableState(extractKey, capacity);
    }

    public bool TryGetValue(string key, int index, int count, out TValue value)
    {
      return this.state.TryGetValue(key, index, count, out value);
    }

    public TValue Add(TValue value)
    {
      TValue newValue;
      while (!this.state.TryAdd(value, out newValue))
      {
        XHashtable<TValue> xhashtable = this;
        bool lockTaken = false;
        try
        {
          Monitor.Enter((object)xhashtable, ref lockTaken);
          XHashtable<TValue>.XHashtableState xhashtableState = this.state.Resize();
          Thread.MemoryBarrier();
          this.state = xhashtableState;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object)xhashtable);
        }
      }
      return newValue;
    }

    public delegate string ExtractKeyDelegate(TValue value);

    private sealed class XHashtableState
    {
      private int[] buckets;
      private XHashtable<TValue>.XHashtableState.Entry[] entries;
      private int numEntries;
      private XHashtable<TValue>.ExtractKeyDelegate extractKey;
      private const int EndOfList = 0;
      private const int FullList = -1;

      public XHashtableState(XHashtable<TValue>.ExtractKeyDelegate extractKey, int capacity)
      {
        this.buckets = new int[capacity];
        this.entries = new XHashtable<TValue>.XHashtableState.Entry[capacity];
        this.extractKey = extractKey;
      }

      public XHashtable<TValue>.XHashtableState Resize()
      {
        if (this.numEntries < this.buckets.Length)
          return this;
        int num = 0;
        for (int index1 = 0; index1 < this.buckets.Length; ++index1)
        {
          int index2 = this.buckets[index1];
          if (index2 == 0)
            index2 = Interlocked.CompareExchange(ref this.buckets[index1], -1, 0);
          for (; index2 > 0; index2 = this.entries[index2].Next != 0 ? this.entries[index2].Next : Interlocked.CompareExchange(ref this.entries[index2].Next, -1, 0))
          {
            if (this.extractKey(this.entries[index2].Value) != null)
              ++num;
          }
        }
        int capacity;
        if (num < this.buckets.Length / 2)
        {
          capacity = this.buckets.Length;
        }
        else
        {
          capacity = this.buckets.Length * 2;
          if (capacity < 0)
            throw new OverflowException();
        }
        XHashtable<TValue>.XHashtableState xhashtableState = new XHashtable<TValue>.XHashtableState(this.extractKey, capacity);
        for (int index1 = 0; index1 < this.buckets.Length; ++index1)
        {
          for (int index2 = this.buckets[index1]; index2 > 0; index2 = this.entries[index2].Next)
          {
            TValue newValue;
            xhashtableState.TryAdd(this.entries[index2].Value, out newValue);
          }
        }
        return xhashtableState;
      }

      public bool TryGetValue(string key, int index, int count, out TValue value)
      {
        int hashCode = XHashtable<TValue>.XHashtableState.ComputeHashCode(key, index, count);
        int entryIndex = 0;
        if (this.FindEntry(hashCode, key, index, count, ref entryIndex))
        {
          value = this.entries[entryIndex].Value;
          return true;
        }
        value = default(TValue);
        return false;
      }

      public bool TryAdd(TValue value, out TValue newValue)
      {
        newValue = value;
        string key = this.extractKey(value);
        if (key == null)
          return true;
        int hashCode = XHashtable<TValue>.XHashtableState.ComputeHashCode(key, 0, key.Length);
        int index = Interlocked.Increment(ref this.numEntries);
        if (index < 0 || index >= this.buckets.Length)
          return false;
        this.entries[index].Value = value;
        this.entries[index].HashCode = hashCode;
        Thread.MemoryBarrier();
        int entryIndex = 0;
        while (!this.FindEntry(hashCode, key, 0, key.Length, ref entryIndex))
        {
          entryIndex = entryIndex != 0 ? Interlocked.CompareExchange(ref this.entries[entryIndex].Next, index, 0) : Interlocked.CompareExchange(ref this.buckets[hashCode & this.buckets.Length - 1], index, 0);
          if (entryIndex <= 0)
            return entryIndex == 0;
        }
        newValue = this.entries[entryIndex].Value;
        return true;
      }

      private bool FindEntry(int hashCode, string key, int index, int count, ref int entryIndex)
      {
        int index1 = entryIndex;
        int index2 = index1 != 0 ? index1 : this.buckets[hashCode & this.buckets.Length - 1];
        while (index2 > 0)
        {
          if (this.entries[index2].HashCode == hashCode)
          {
            string strB = this.extractKey(this.entries[index2].Value);
            if (strB == null)
            {
              if (this.entries[index2].Next > 0)
              {
                this.entries[index2].Value = default(TValue);
                index2 = this.entries[index2].Next;
                if (index1 == 0)
                {
                  this.buckets[hashCode & this.buckets.Length - 1] = index2;
                  continue;
                }
                this.entries[index1].Next = index2;
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
          index2 = this.entries[index2].Next;
        }
        entryIndex = index1;
        return false;
      }

      private static int ComputeHashCode(string key, int index, int count)
      {
        int num1 = 352654597;
        int num2 = index + count;
        for (int index1 = index; index1 < num2; ++index1)
          num1 += num1 << 7 ^ (int)key[index1];
        int num3 = num1 - (num1 >> 17);
        int num4 = num3 - (num3 >> 11);
        return num4 - (num4 >> 5) & int.MaxValue;
      }

      private struct Entry
      {
        public TValue Value;
        public int HashCode;
        public int Next;
      }
    }
  }
}
