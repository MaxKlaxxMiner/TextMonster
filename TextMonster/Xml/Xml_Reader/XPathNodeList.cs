using System;
using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XPathNodeList : XmlNodeList
  {
    List<XmlNode> list;
    XPathNodeIterator nodeIterator;
    bool done;

    public XPathNodeList(XPathNodeIterator nodeIterator)
    {
      this.nodeIterator = nodeIterator;
      this.list = new List<XmlNode>();
      this.done = false;
    }

    public override int Count
    {
      get
      {
        if (!done)
        {
          ReadUntil(Int32.MaxValue);
        }
        return list.Count;
      }
    }

    private XmlNode GetNode(XPathNavigator n)
    {
      IHasXmlNode iHasNode = (IHasXmlNode)n;
      return iHasNode.GetNode();
    }

    internal int ReadUntil(int index)
    {
      int count = list.Count;
      while (!done && count <= index)
      {
        if (nodeIterator.MoveNext())
        {
          XmlNode n = GetNode(nodeIterator.Current);
          if (n != null)
          {
            list.Add(n);
            count++;
          }
        }
        else
        {
          done = true;
          break;
        }
      }
      return count;
    }

    public override XmlNode Item(int index)
    {
      if (list.Count <= index)
      {
        ReadUntil(index);
      }
      if (index < 0 || list.Count <= index)
      {
        return null;
      }
      return list[index];
    }

    public override IEnumerator GetEnumerator()
    {
      return new XmlNodeListEnumerator(this);
    }
  }
}
