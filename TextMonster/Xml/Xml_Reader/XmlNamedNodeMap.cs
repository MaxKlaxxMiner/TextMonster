using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents a collection of nodes that can be accessed by name or index.
  public partial class XmlNamedNodeMap : IEnumerable
  {
    internal XmlNode parent;
    internal SmallXmlNodeList nodes;

    internal XmlNamedNodeMap(XmlNode parent)
    {
      this.parent = parent;
    }

    // Retrieves a XmlNode specified by name.
    public virtual XmlNode GetNamedItem(String name)
    {
      int offset = FindNodeOffset(name);
      if (offset >= 0)
        return (XmlNode)nodes[offset];
      return null;
    }

    // Adds a XmlNode using its Name property
    public virtual XmlNode SetNamedItem(XmlNode node)
    {
      if (node == null)
        return null;

      int offset = FindNodeOffset(node.LocalName, node.NamespaceURI);
      if (offset == -1)
      {
        AddNode(node);
        return null;
      }
      else
      {
        return ReplaceNodeAt(offset, node);
      }
    }

    // Removes the node specified by name.
    public virtual XmlNode RemoveNamedItem(String name)
    {
      int offset = FindNodeOffset(name);
      if (offset >= 0)
      {
        return RemoveNodeAt(offset);
      }
      return null;
    }

    // Gets the number of nodes in this XmlNamedNodeMap.
    public virtual int Count
    {
      get
      {
        return nodes.Count;
      }
    }

    public virtual IEnumerator GetEnumerator()
    {
      return nodes.GetEnumerator();
    }

    internal int FindNodeOffset(string name)
    {
      int c = this.Count;
      for (int i = 0; i < c; i++)
      {
        XmlNode node = (XmlNode)nodes[i];

        if (name == node.Name)
          return i;
      }

      return -1;
    }

    internal int FindNodeOffset(string localName, string namespaceURI)
    {
      int c = this.Count;
      for (int i = 0; i < c; i++)
      {
        XmlNode node = (XmlNode)nodes[i];

        if (node.LocalName == localName && node.NamespaceURI == namespaceURI)
          return i;
      }

      return -1;
    }

    internal virtual XmlNode AddNode(XmlNode node)
    {
      XmlNode oldParent;
      if (node.NodeType == XmlNodeType.Attribute)
        oldParent = ((XmlAttribute)node).OwnerElement;
      else
        oldParent = node.ParentNode;
      string nodeValue = node.Value;
      XmlNodeChangedEventArgs args = parent.GetEventArgs(node, oldParent, parent, nodeValue, nodeValue, XmlNodeChangedAction.Insert);

      if (args != null)
        parent.BeforeEvent(args);

      nodes.Add(node);
      node.SetParent(parent);

      if (args != null)
        parent.AfterEvent(args);

      return node;
    }

    internal virtual XmlNode AddNodeForLoad(XmlNode node, XmlDocument doc)
    {
      XmlNodeChangedEventArgs args = doc.GetInsertEventArgsForLoad(node, parent);
      if (args != null)
      {
        doc.BeforeEvent(args);
      }
      nodes.Add(node);
      node.SetParent(parent);
      if (args != null)
      {
        doc.AfterEvent(args);
      }
      return node;
    }

    internal virtual XmlNode RemoveNodeAt(int i)
    {
      XmlNode oldNode = (XmlNode)nodes[i];

      string oldNodeValue = oldNode.Value;
      XmlNodeChangedEventArgs args = parent.GetEventArgs(oldNode, parent, null, oldNodeValue, oldNodeValue, XmlNodeChangedAction.Remove);

      if (args != null)
        parent.BeforeEvent(args);

      nodes.RemoveAt(i);
      oldNode.SetParent(null);

      if (args != null)
        parent.AfterEvent(args);

      return oldNode;
    }

    internal XmlNode ReplaceNodeAt(int i, XmlNode node)
    {
      XmlNode oldNode = RemoveNodeAt(i);
      InsertNodeAt(i, node);
      return oldNode;
    }

    internal virtual XmlNode InsertNodeAt(int i, XmlNode node)
    {
      XmlNode oldParent;
      if (node.NodeType == XmlNodeType.Attribute)
        oldParent = ((XmlAttribute)node).OwnerElement;
      else
        oldParent = node.ParentNode;

      string nodeValue = node.Value;
      XmlNodeChangedEventArgs args = parent.GetEventArgs(node, oldParent, parent, nodeValue, nodeValue, XmlNodeChangedAction.Insert);

      if (args != null)
        parent.BeforeEvent(args);

      nodes.Insert(i, node);
      node.SetParent(parent);

      if (args != null)
        parent.AfterEvent(args);

      return node;
    }

    // Optimized to minimize space in the zero or one element cases.
    internal struct SmallXmlNodeList
    {

      // If field is null, that represents an empty list.
      // If field is non-null, but not an ArrayList, then the 'list' contains a single
      // object.
      // Otherwise, field is an ArrayList. Once the field upgrades to an ArrayList, it
      // never degrades back, even if all elements are removed.
      private object field;

      public int Count
      {
        get
        {
          if (field == null)
            return 0;

          ArrayList list = field as ArrayList;
          if (list != null)
            return list.Count;

          return 1;
        }
      }

      public object this[int index]
      {
        get
        {
          if (field == null)
            throw new ArgumentOutOfRangeException("index");

          ArrayList list = field as ArrayList;
          if (list != null)
            return list[index];

          if (index != 0)
            throw new ArgumentOutOfRangeException("index");

          return field;
        }
      }

      public void Add(object value)
      {
        if (field == null)
        {
          if (value == null)
          {
            // If a single null value needs to be stored, then
            // upgrade to an ArrayList
            ArrayList temp = new ArrayList();
            temp.Add(null);
            field = temp;
          }
          else
            field = value;

          return;
        }

        ArrayList list = field as ArrayList;
        if (list != null)
        {
          list.Add(value);
        }
        else
        {
          list = new ArrayList();
          list.Add(field);
          list.Add(value);
          field = list;
        }
      }

      public void RemoveAt(int index)
      {
        if (field == null)
          throw new ArgumentOutOfRangeException("index");

        ArrayList list = field as ArrayList;
        if (list != null)
        {
          list.RemoveAt(index);
          return;
        }

        if (index != 0)
          throw new ArgumentOutOfRangeException("index");

        field = null;
      }

      public void Insert(int index, object value)
      {
        if (field == null)
        {
          if (index != 0)
            throw new ArgumentOutOfRangeException("index");
          Add(value);
          return;
        }

        ArrayList list = field as ArrayList;
        if (list != null)
        {
          list.Insert(index, value);
          return;
        }

        if (index == 0)
        {
          list = new ArrayList();
          list.Add(value);
          list.Add(field);
          field = list;
        }
        else if (index == 1)
        {
          list = new ArrayList();
          list.Add(field);
          list.Add(value);
          field = list;
        }
        else
        {
          throw new ArgumentOutOfRangeException("index");
        }
      }

      class SingleObjectEnumerator : IEnumerator
      {
        object loneValue;
        int position = -1;

        public SingleObjectEnumerator(object value)
        {
          loneValue = value;
        }

        public object Current
        {
          get
          {
            if (position != 0)
            {
              throw new InvalidOperationException();
            }
            return this.loneValue;
          }
        }

        public bool MoveNext()
        {
          if (position < 0)
          {
            position = 0;
            return true;
          }
          position = 1;
          return false;
        }

        public void Reset()
        {
          position = -1;
        }
      }

      public IEnumerator GetEnumerator()
      {
        if (field == null)
        {
          return XmlDocument.EmptyEnumerator;
        }

        ArrayList list = field as ArrayList;
        if (list != null)
        {
          return list.GetEnumerator();
        }

        return new SingleObjectEnumerator(field);
      }
    }
  }
}
