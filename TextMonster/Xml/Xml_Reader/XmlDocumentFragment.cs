using System;

namespace TextMonster.Xml.Xml_Reader
{
  public class XmlDocumentFragment : XmlNode
  {
    XmlLinkedNode lastChild;

    protected internal XmlDocumentFragment(XmlDocument ownerDocument)
    {
      if (ownerDocument == null)
        throw new ArgumentException(Res.GetString(Res.Xdom_Node_Null_Doc));
      parentNode = ownerDocument;
    }

    // Gets the name of the node.
    public override String Name
    {
      get { return OwnerDocument.strDocumentFragmentName; }
    }

    // Gets the name of the current node without the namespace prefix.
    public override String LocalName
    {
      get { return OwnerDocument.strDocumentFragmentName; }
    }

    // Gets the type of the current node.
    public override XmlNodeType NodeType
    {
      get { return XmlNodeType.DocumentFragment; }
    }

    // Gets the parent of this node (for nodes that can have parents).
    public override XmlNode ParentNode
    {
      get { return null; }
    }

    // Gets the XmlDocument that contains this node.
    public override XmlDocument OwnerDocument
    {
      get
      {
        return (XmlDocument)parentNode;
      }

    }

    public override XmlNode CloneNode(bool deep)
    {
      XmlDocument doc = OwnerDocument;
      XmlDocumentFragment clone = doc.CreateDocumentFragment();
      if (deep)
        clone.CopyChildren(doc, this, deep);
      return clone;
    }

    internal override bool IsContainer
    {
      get { return true; }
    }

    internal override XmlLinkedNode LastNode
    {
      get { return lastChild; }
      set { lastChild = value; }
    }

    internal override bool IsValidChildType(XmlNodeType type)
    {
      switch (type)
      {
        case XmlNodeType.Element:
        case XmlNodeType.Text:
        case XmlNodeType.EntityReference:
        case XmlNodeType.Comment:
        case XmlNodeType.Whitespace:
        case XmlNodeType.SignificantWhitespace:
        case XmlNodeType.ProcessingInstruction:
        case XmlNodeType.CDATA:
          return true;

        case XmlNodeType.XmlDeclaration:
          //if there is an XmlDeclaration node, it has to be the first node;
          XmlNode firstNode = FirstChild;
          if (firstNode == null || firstNode.NodeType != XmlNodeType.XmlDeclaration)
            return true;
          else
            return false; //not allowed to insert a second XmlDeclaration node
        default:
          return false;
      }
    }
    internal override bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
    {
      if (newChild.NodeType == XmlNodeType.XmlDeclaration)
      {
        if (refChild == null)
        {
          //append at the end
          return (LastNode == null);
        }
        else
          return false;
      }
      return true;
    }

    internal override bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
    {
      if (newChild.NodeType == XmlNodeType.XmlDeclaration)
      {
        return (refChild == null || refChild == FirstChild);
      }
      return true;
    }

    // Saves the node to the specified XmlWriter.
    public override void WriteTo(XmlWriter w)
    {
      WriteContentTo(w);
    }

    // Saves all the children of the node to the specified XmlWriter.
    public override void WriteContentTo(XmlWriter w)
    {
      foreach (XmlNode n in this)
      {
        n.WriteTo(w);
      }
    }

    internal override XPathNodeType XPNodeType { get { return XPathNodeType.Root; } }
  }
}