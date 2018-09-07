﻿using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents an ordered collection of nodes.
  public abstract class XmlNodeList : IEnumerable, IDisposable
  {

    // Retrieves a node at the given index.
    public abstract XmlNode Item(int index);

    // Gets the number of nodes in this XmlNodeList.
    public abstract int Count { get; }

    // Provides a simple ForEach-style iteration over the collection of nodes in
    // this XmlNodeList.
    public abstract IEnumerator GetEnumerator();

    // Retrieves a node at the given index.
    [System.Runtime.CompilerServices.IndexerName("ItemOf")]
    public XmlNode this[int i] { get { return Item(i); } }

    void IDisposable.Dispose()
    {
      PrivateDisposeNodeList();
    }

    protected virtual void PrivateDisposeNodeList() { }
  }
}
