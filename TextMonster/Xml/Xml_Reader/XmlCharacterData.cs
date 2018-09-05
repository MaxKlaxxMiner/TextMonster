using System;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  // Provides text-manipulation methods that are used by several classes.
  public abstract class XmlCharacterData : XmlLinkedNode
  {
    string data;

    //base(doc) will throw exception if doc is null.
    protected internal XmlCharacterData(string data, XmlDocument doc)
      : base(doc)
    {
      this.data = data;
    }

    // Gets or sets the value of the node.
    public override String Value
    {
      get { return Data; }
      set { Data = value; }
    }

    // Gets or sets the concatenated values of the node and
    // all its children.
    public override string InnerText
    {
      get { return Value; }
      set { Value = value; }
    }

    // Contains this node's data.
    public virtual string Data
    {
      get
      {
        if (data != null)
        {
          return data;
        }
        else
        {
          return String.Empty;
        }
      }

      set
      {
        XmlNode parent = ParentNode;
        XmlNodeChangedEventArgs args = GetEventArgs(this, parent, parent, this.data, value, XmlNodeChangedAction.Change);

        if (args != null)
          BeforeEvent(args);

        data = value;

        if (args != null)
          AfterEvent(args);
      }
    }

    // Gets the length of the data, in characters.
    public virtual int Length
    {
      get
      {
        if (data != null)
        {
          return data.Length;
        }
        return 0;
      }
    }

    // Retrieves a substring of the full string from the specified range.
    public virtual String Substring(int offset, int count)
    {
      int len = data != null ? data.Length : 0;
      if (len > 0)
      {
        if (len < (offset + count))
        {
          count = len - offset;
        }
        return data.Substring(offset, count);
      }
      return String.Empty;
    }

    public virtual void DeleteData(int offset, int count)
    {
      int len = data != null ? data.Length : 0;
      if (len > 0)
      {
        if (len < (offset + count))
        {
          count = Math.Max(len - offset, 0);
        }
      }

      string newValue = new StringBuilder(data).Remove(offset, count).ToString();
      XmlNode parent = ParentNode;
      XmlNodeChangedEventArgs args = GetEventArgs(this, parent, parent, data, newValue, XmlNodeChangedAction.Change);

      if (args != null)
        BeforeEvent(args);

      this.data = newValue;

      if (args != null)
        AfterEvent(args);
    }

    // Replace the specified number of characters starting at the specified offset with the
    // specified string.
    public virtual void ReplaceData(int offset, int count, String strData)
    {
      //Debug.Assert(offset >= 0 && offset <= Length);

      int len = data != null ? data.Length : 0;
      if (len > 0)
      {
        if (len < (offset + count))
        {
          count = Math.Max(len - offset, 0);
        }
      }

      StringBuilder temp = new StringBuilder(data).Remove(offset, count);
      string newValue = temp.Insert(offset, strData).ToString();

      XmlNode parent = ParentNode;
      XmlNodeChangedEventArgs args = GetEventArgs(this, parent, parent, data, newValue, XmlNodeChangedAction.Change);

      if (args != null)
        BeforeEvent(args);

      this.data = newValue;

      if (args != null)
        AfterEvent(args);
    }

    internal bool CheckOnData(string data)
    {
      return XmlCharType.Instance.IsOnlyWhitespace(data);
    }

    internal bool DecideXPNodeTypeForTextNodes(XmlNode node, ref XPathNodeType xnt)
    {
      //returns true - if all siblings of the node are processed else returns false.
      //The reference XPathNodeType argument being passed in is the watermark that
      //changes according to the siblings nodetype and will contain the correct
      //nodetype when it returns.

      while (node != null)
      {
        switch (node.NodeType)
        {
          case XmlNodeType.Whitespace:
          break;
          case XmlNodeType.SignificantWhitespace:
          xnt = XPathNodeType.SignificantWhitespace;
          break;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          xnt = XPathNodeType.Text;
          return false;
          case XmlNodeType.EntityReference:
          if (!DecideXPNodeTypeForTextNodes(node.FirstChild, ref xnt))
          {
            return false;
          }
          break;
          default:
          return false;
        }
        node = node.NextSibling;
      }
      return true;
    }
  }
}
