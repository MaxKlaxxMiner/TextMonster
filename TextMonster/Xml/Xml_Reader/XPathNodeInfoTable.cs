using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// An atomization table for XPathNodeInfoAtom.
  /// </summary>
  sealed internal class XPathNodeInfoTable
  {
    private XPathNodeInfoAtom[] hashTable;
    private int sizeTable;
    private XPathNodeInfoAtom infoCached;

#if DEBUG
        private const int DefaultTableSize = 2;
#else
    private const int DefaultTableSize = 32;
#endif

    /// <summary>
    /// Constructor.
    /// </summary>
    public XPathNodeInfoTable()
    {
      this.hashTable = new XPathNodeInfoAtom[DefaultTableSize];
      this.sizeTable = 0;
    }

    /// <summary>
    /// Create a new XNodeInfoAtom and ensure it is atomized in the table.
    /// </summary>
    public XPathNodeInfoAtom Create(string localName, string namespaceUri, string prefix, string baseUri,
                                      XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar,
                                      XPathDocument doc, int lineNumBase, int linePosBase)
    {
      XPathNodeInfoAtom info;

      // If this.infoCached already exists, then reuse it; else create new InfoAtom
      if (this.infoCached == null)
      {
        info = new XPathNodeInfoAtom(localName, namespaceUri, prefix, baseUri,
                                     pageParent, pageSibling, pageSimilar,
                                     doc, lineNumBase, linePosBase);
      }
      else
      {
        info = this.infoCached;
        this.infoCached = info.Next;

        info.Init(localName, namespaceUri, prefix, baseUri,
                  pageParent, pageSibling, pageSimilar,
                  doc, lineNumBase, linePosBase);
      }

      return Atomize(info);
    }


    /// <summary>
    /// Add a shared information item to the atomization table.  If a matching item already exists, then that
    /// instance is returned.  Otherwise, a new item is created.  Thus, if itemX and itemY have both been added
    /// to the same InfoTable:
    /// 1. itemX.Equals(itemY) != true
    /// 2. (object) itemX != (object) itemY
    /// </summary>
    private XPathNodeInfoAtom Atomize(XPathNodeInfoAtom info)
    {
      XPathNodeInfoAtom infoNew, infoNext;

      // Search for existing XNodeInfoAtom in the table
      infoNew = this.hashTable[info.GetHashCode() & (this.hashTable.Length - 1)];
      while (infoNew != null)
      {
        if (info.Equals(infoNew))
        {
          // Found existing atom, so return that.  Reuse "info".
          info.Next = this.infoCached;
          this.infoCached = info;
          return infoNew;
        }
        infoNew = infoNew.Next;
      }

      // Expand table and rehash if necessary
      if (this.sizeTable >= this.hashTable.Length)
      {
        XPathNodeInfoAtom[] oldTable = this.hashTable;
        this.hashTable = new XPathNodeInfoAtom[oldTable.Length * 2];

        for (int i = 0; i < oldTable.Length; i++)
        {
          infoNew = oldTable[i];
          while (infoNew != null)
          {
            infoNext = infoNew.Next;
            AddInfo(infoNew);
            infoNew = infoNext;
          }
        }
      }

      // Can't find an existing XNodeInfoAtom, so use the one that was passed in
      AddInfo(info);

      return info;
    }

    /// <summary>
    /// Add a previously constructed InfoAtom to the table.  If a collision occurs, then insert "info"
    /// as the head of a linked list.
    /// </summary>
    private void AddInfo(XPathNodeInfoAtom info)
    {
      int idx = info.GetHashCode() & (this.hashTable.Length - 1);
      info.Next = this.hashTable[idx];
      this.hashTable[idx] = info;
      this.sizeTable++;
    }

    /// <summary>
    /// Return InfoAtomTable formatted as a string.
    /// </summary>
    public override string ToString()
    {
      StringBuilder bldr = new StringBuilder();
      XPathNodeInfoAtom infoAtom;

      for (int i = 0; i < this.hashTable.Length; i++)
      {
        bldr.AppendFormat("{0,4}: ", i);

        infoAtom = this.hashTable[i];

        while (infoAtom != null)
        {
          if ((object)infoAtom != (object)this.hashTable[i])
            bldr.Append("\n      ");

          bldr.Append(infoAtom);

          infoAtom = infoAtom.Next;
        }

        bldr.Append('\n');
      }

      return bldr.ToString();
    }
  }
}
