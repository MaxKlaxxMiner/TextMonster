using System;
using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaObjectTable.uex' path='docs/doc[@for="XmlSchemaObjectTable"]/*' />
  public class XmlSchemaObjectTable
  {
    Dictionary<XmlQualifiedName, XmlSchemaObject> table = new Dictionary<XmlQualifiedName, XmlSchemaObject>();
    List<XmlSchemaObjectEntry> entries = new List<XmlSchemaObjectEntry>();

    internal XmlSchemaObjectTable()
    {
    }

    internal void Add(XmlQualifiedName name, XmlSchemaObject value)
    {
      table.Add(name, value);
      entries.Add(new XmlSchemaObjectEntry(name, value));
    }

    internal void Insert(XmlQualifiedName name, XmlSchemaObject value)
    {
      XmlSchemaObject oldValue = null;
      if (table.TryGetValue(name, out oldValue))
      {
        table[name] = value; //set new value
        int matchedIndex = FindIndexByValue(oldValue);
        //set new entry
        entries[matchedIndex] = new XmlSchemaObjectEntry(name, value);
      }
      else
      {
        Add(name, value);
      }

    }

    internal void Clear()
    {
      table.Clear();
      entries.Clear();
    }

    internal void Remove(XmlQualifiedName name)
    {
      XmlSchemaObject value;
      if (table.TryGetValue(name, out value))
      {
        table.Remove(name);
        int matchedIndex = FindIndexByValue(value);
        entries.RemoveAt(matchedIndex);
      }
    }

    private int FindIndexByValue(XmlSchemaObject xso)
    {
      int index;
      for (index = 0; index < entries.Count; index++)
      {
        if ((object)entries[index].xso == (object)xso)
        {
          return index;
        }
      }
      return -1;
    }
    /// <include file='doc\XmlSchemaObjectTable.uex' path='docs/doc[@for="XmlSchemaObjectTable.Count"]/*' />
    public int Count
    {
      get
      {
        return table.Count;
      }
    }

    /// <include file='doc\XmlSchemaObjectTable.uex' path='docs/doc[@for="XmlSchemaObjectTable.Contains"]/*' />
    public bool Contains(XmlQualifiedName name)
    {
      return table.ContainsKey(name);
    }

    /// <include file='doc\XmlSchemaObjectTable.uex' path='docs/doc[@for="XmlSchemaObjectTable.this"]/*' />
    public XmlSchemaObject this[XmlQualifiedName name]
    {
      get
      {
        XmlSchemaObject value;
        if (table.TryGetValue(name, out value))
        {
          return value;
        }
        return null;
      }
    }

    /// <include file='doc\XmlSchemaObjectTable.uex' path='docs/doc[@for="XmlSchemaObjectTable.Values"]/*' />
    public ICollection Values
    {
      get
      {
        return new ValuesCollection(entries, table.Count);
      }
    }

    internal enum EnumeratorType
    {
      Keys,
      Values,
      DictionaryEntry,
    }

    internal struct XmlSchemaObjectEntry
    {
      internal XmlQualifiedName qname;
      internal XmlSchemaObject xso;

      public XmlSchemaObjectEntry(XmlQualifiedName name, XmlSchemaObject value)
      {
        qname = name;
        xso = value;
      }
    }

    //ICollection for Values 
    internal class ValuesCollection : ICollection
    {
      private List<XmlSchemaObjectEntry> entries;
      int size;

      internal ValuesCollection(List<XmlSchemaObjectEntry> entries, int size)
      {
        this.entries = entries;
        this.size = size;
      }

      public int Count
      {
        get { return size; }
      }

      public Object SyncRoot
      {
        get
        {
          return ((ICollection)entries).SyncRoot;
        }
      }

      public bool IsSynchronized
      {
        get
        {
          return ((ICollection)entries).IsSynchronized;
        }
      }

      public void CopyTo(Array array, int arrayIndex)
      {
        if (array == null)
          throw new ArgumentNullException("array");

        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException("arrayIndex");

        for (int i = 0; i < size; i++)
        {
          array.SetValue(entries[i].xso, arrayIndex++);
        }
      }

      public IEnumerator GetEnumerator()
      {
        return new XSOEnumerator(this.entries, this.size, EnumeratorType.Values);
      }
    }

    internal class XSOEnumerator : IEnumerator
    {
      private List<XmlSchemaObjectEntry> entries;
      private EnumeratorType enumType;

      protected int currentIndex;
      protected int size;
      protected XmlQualifiedName currentKey;
      protected XmlSchemaObject currentValue;


      internal XSOEnumerator(List<XmlSchemaObjectEntry> entries, int size, EnumeratorType enumType)
      {
        this.entries = entries;
        this.size = size;
        this.enumType = enumType;
        currentIndex = -1;
      }

      public Object Current
      {
        get
        {
          if (currentIndex == -1)
          {
            throw new InvalidOperationException(Res.GetString(Res.Sch_EnumNotStarted, string.Empty));
          }
          if (currentIndex >= size)
          {
            throw new InvalidOperationException(Res.GetString(Res.Sch_EnumFinished, string.Empty));
          }
          switch (enumType)
          {
            case EnumeratorType.Keys:
            return currentKey;

            case EnumeratorType.Values:
            return currentValue;

            case EnumeratorType.DictionaryEntry:
            return new DictionaryEntry(currentKey, currentValue);

            default:
            break;
          }
          return null;
        }
      }

      public bool MoveNext()
      {
        if (currentIndex >= size - 1)
        {
          currentValue = null;
          currentKey = null;
          return false;
        }
        currentIndex++;
        currentValue = entries[currentIndex].xso;
        currentKey = entries[currentIndex].qname;
        return true;
      }

      public void Reset()
      {
        currentIndex = -1;
        currentValue = null;
        currentKey = null;
      }
    }
  }
}
