using System;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents the text content of an element or attribute.
  public class XmlText : XmlCharacterData
  {
    internal XmlText(string strData)
      : this(strData, null)
    {
    }

    protected internal XmlText(string strData, XmlDocument doc)
      : base(strData, doc)
    {
    }

    // Gets the name of the node.
    public override String Name
    {
      get
      {
        return OwnerDocument.strTextName;
      }
    }

    // Gets the name of the current node without the namespace prefix.
    public override String LocalName
    {
      get
      {
        return OwnerDocument.strTextName;
      }
    }

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Text;
      }
    }

    public override XmlNode ParentNode
    {
      get
      {
        switch (parentNode.NodeType)
        {
          case XmlNodeType.Document:
          return null;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          XmlNode parent = parentNode.parentNode;
          while (parent.IsText)
          {
            parent = parent.parentNode;
          }
          return parent;
          default:
          return parentNode;
        }
      }
    }

    // Creates a duplicate of this node.
    public override XmlNode CloneNode(bool deep)
    {
      return OwnerDocument.CreateTextNode(Data);
    }

    public override String Value
    {
      get
      {
        return Data;
      }

      set
      {
        Data = value;
        XmlNode parent = parentNode;
        if (parent != null && parent.NodeType == XmlNodeType.Attribute)
        {
          XmlUnspecifiedAttribute attr = parent as XmlUnspecifiedAttribute;
          if (attr != null && !attr.Specified)
          {
            attr.SetSpecified(true);
          }
        }
      }
    }

    // Saves the node to the specified XmlWriter.
    public override void WriteTo(XmlWriter w)
    {
      w.WriteString(Data);
    }

    internal override bool IsText
    {
      get
      {
        return true;
      }
    }
  }
}
