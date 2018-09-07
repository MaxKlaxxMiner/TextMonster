using System;

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
    public string Data
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

    internal bool CheckOnData(string data)
    {
      return XmlCharType.Instance.IsOnlyWhitespace(data);
    }
  }
}
