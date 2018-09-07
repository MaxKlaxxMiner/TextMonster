using System;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  public class NameTable
  {
    public string Add(string name)
    {
      return name;
    }

    sealed class StringElement
    {
      public readonly string value;
      public readonly StringElement next;
      public readonly ulong crc64;

      public StringElement(StringElement parent, string value)
      {
        this.value = value;
        crc64 = value.GetCrc64();
        next = parent;
      }
    }

    readonly StringElement[] dict = new StringElement[256];

    public string Add(char[] chars, int ofs, int len)
    {
      ulong crc = chars.GetCrc64(ofs, len);

      for (var el = dict[(byte)len]; el != null; el = el.next)
      {
        if (el.crc64 == crc) return el.value;
      }

      string name = new string(chars, ofs, len);

      dict[(byte)len] = new StringElement(dict[(byte)len], name);

      return name;
    }

    public string Get(string name)
    {
      ulong crc = name.GetCrc64();

      for (var el = dict[(byte)name.Length]; el != null; el = el.next)
      {
        if (el.crc64 == crc) return el.value;
      }

      return name;
    }
  }

  /// <include file='doc\NameTable.uex' path='docs/doc[@for="NameTable"]/*' />
  /// <devdoc>
  ///    <para>
  ///       NameTable implemented as a simple hash table.
  ///    </para>
  /// </devdoc>
  public class NameTableOld
  {
    class Entry
    {
      internal string str;
      internal int hashCode;
      internal Entry next;

      internal Entry(string str, int hashCode, Entry next)
      {
        this.str = str;
        this.hashCode = hashCode;
        this.next = next;
      }
    }

    Entry[] entries;
    int count;
    int mask;
    int hashCodeRandomizer;

    /// <include file='doc\NameTable.uex' path='docs/doc[@for="NameTable.NameTable"]/*' />
    /// <devdoc>
    ///      Public constructor.
    /// </devdoc>
    public NameTableOld()
    {
      mask = 31;
      entries = new Entry[mask + 1];
      hashCodeRandomizer = Environment.TickCount;
    }

    /// <include file='doc\NameTable.uex' path='docs/doc[@for="NameTable.Add"]/*' />
    /// <devdoc>
    ///      Add the given string to the NameTable or return
    ///      the existing string if it is already in the NameTable.
    /// </devdoc>
    public string Add(string key)
    {
      if (key == null)
      {
        throw new ArgumentNullException("key");
      }
      int len = key.Length;
      if (len == 0)
      {
        return string.Empty;
      }

      int hashCode = ComputeHash32(key);

      for (Entry e = entries[hashCode & mask]; e != null; e = e.next)
      {
        if (e.hashCode == hashCode && e.str.Equals(key))
        {
          return e.str;
        }
      }
      return AddEntry(key, hashCode);
    }

    /// <include file='doc\NameTable.uex' path='docs/doc[@for="NameTable.Add1"]/*' />
    /// <devdoc>
    ///      Add the given string to the NameTable or return
    ///      the existing string if it is already in the NameTable.
    /// </devdoc>
    public string Add(char[] key, int start, int len)
    {
      if (len == 0)
      {
        return string.Empty;
      }

      // Compatibility check to ensure same exception as previous versions
      // independently of any exceptions throw by the hashing function.
      // note that NullReferenceException is the first one if key is null.
      if (start >= key.Length || start < 0 || (long)start + len > (long)key.Length)
      {
        throw new IndexOutOfRangeException();
      }

      // Compatibility check for len < 0, just throw the same exception as new string(key, start, len)
      if (len < 0)
      {
        throw new ArgumentOutOfRangeException();
      }

      int hashCode = ComputeHash32(key, start, len);

      for (Entry e = entries[hashCode & mask]; e != null; e = e.next)
      {
        if (e.hashCode == hashCode && TextEquals(e.str, key, start, len))
        {
          return e.str;
        }
      }
      return AddEntry(new string(key, start, len), hashCode);
    }

    /// <include file='doc\NameTable.uex' path='docs/doc[@for="NameTable.Get"]/*' />
    /// <devdoc>
    ///      Find the matching string in the NameTable.
    /// </devdoc>
    public string Get(string value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      if (value.Length == 0)
      {
        return string.Empty;
      }

      int hashCode = ComputeHash32(value);

      for (Entry e = entries[hashCode & mask]; e != null; e = e.next)
      {
        if (e.hashCode == hashCode && e.str.Equals(value))
        {
          return e.str;
        }
      }
      return null;
    }

    private string AddEntry(string str, int hashCode)
    {
      int index = hashCode & mask;
      Entry e = new Entry(str, hashCode, entries[index]);
      entries[index] = e;
      if (count++ == mask)
      {
        Grow();
      }
      return e.str;
    }

    private void Grow()
    {
      int newMask = mask * 2 + 1;
      Entry[] oldEntries = entries;
      Entry[] newEntries = new Entry[newMask + 1];

      // use oldEntries.Length to eliminate the rangecheck            
      for (int i = 0; i < oldEntries.Length; i++)
      {
        Entry e = oldEntries[i];
        while (e != null)
        {
          int newIndex = e.hashCode & newMask;
          Entry tmp = e.next;
          e.next = newEntries[newIndex];
          newEntries[newIndex] = e;
          e = tmp;
        }
      }

      entries = newEntries;
      mask = newMask;
    }

    private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
    {
      if (str1.Length != str2Length)
      {
        return false;
      }
      // use array.Length to eliminate the rangecheck
      for (int i = 0; i < str1.Length; i++)
      {
        if (str1[i] != str2[str2Start + i])
        {
          return false;
        }
      }
      return true;
    }

    // Marvin hash is not being added to Silverlight keep on legacy hashing
    private int ComputeHash32(string key)
    {
      int hashCode = key.Length + hashCodeRandomizer;
      // use key.Length to eliminate the rangecheck
      for (int i = 0; i < key.Length; i++)
      {
        hashCode += (hashCode << 7) ^ key[i];
      }
      // mix it a bit more
      hashCode -= hashCode >> 17;
      hashCode -= hashCode >> 11;
      hashCode -= hashCode >> 5;

      return hashCode;
    }

    private int ComputeHash32(char[] key, int start, int len)
    {
      int hashCode = len + hashCodeRandomizer;
      hashCode += (hashCode << 7) ^ key[start];   // this will throw IndexOutOfRangeException in case the start index is invalid
      int end = start + len;
      for (int i = start + 1; i < end; i++)
      {
        hashCode += (hashCode << 7) ^ key[i];
      }
      // mix it a bit more
      hashCode -= hashCode >> 17;
      hashCode -= hashCode >> 11;
      hashCode -= hashCode >> 5;

      return hashCode;
    }
  }
}
