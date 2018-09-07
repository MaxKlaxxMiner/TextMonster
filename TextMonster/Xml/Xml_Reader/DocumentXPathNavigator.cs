using System;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class DocumentXPathNavigator : XPathNavigator, IHasXmlNode
  {
    private XmlDocument document; // owner document
    private XmlNode source; // navigator position 
    private int attributeIndex; // index in attribute collection for attribute 
    private XmlElement namespaceParent; // parent for namespace

    public DocumentXPathNavigator(XmlDocument document, XmlNode node)
    {
      this.document = document;
      ResetPosition(node);
    }

    public DocumentXPathNavigator(DocumentXPathNavigator other)
    {
      document = other.document;
      source = other.source;
      attributeIndex = other.attributeIndex;
      namespaceParent = other.namespaceParent;
    }

    public override XPathNavigator Clone()
    {
      return new DocumentXPathNavigator(this);
    }

    public override XmlNameTable NameTable
    {
      get
      {
        return document.NameTable;
      }
    }

    public override XPathNodeType NodeType
    {
      get
      {
        CalibrateText();

        return (XPathNodeType)source.XPNodeType;
      }
    }

    public override string LocalName
    {
      get
      {
        return source.XPLocalName;
      }
    }

    public override string NamespaceURI
    {
      get
      {
        XmlAttribute attribute = source as XmlAttribute;
        if (attribute != null
            && attribute.IsNamespace)
        {
          return string.Empty;
        }
        return source.NamespaceURI;
      }
    }

    public override string Name
    {
      get
      {
        switch (source.NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.ProcessingInstruction:
            return source.Name;
          case XmlNodeType.Attribute:
            if (((XmlAttribute)source).IsNamespace)
            {
              string localName = source.LocalName;
              if (Ref.Equal(localName, document.strXmlns))
              {
                return string.Empty; // xmlns declaration
              }
              return localName; // xmlns:name declaration
            }
            return source.Name; // attribute  
          default:
            return string.Empty;
        }
      }
    }

    public override string Value
    {
      get
      {
        switch (source.NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.DocumentFragment:
            return source.InnerText;
          case XmlNodeType.Document:
            return ValueDocument;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
            return ValueText;
          default:
            return source.Value;
        }
      }
    }

    private string ValueDocument
    {
      get
      {
        XmlElement element = document.DocumentElement;
        if (element != null)
        {
          return element.InnerText;
        }
        return string.Empty;
      }
    }

    private string ValueText
    {
      get
      {
        CalibrateText();

        string value = source.Value;
        XmlNode nextSibling = NextSibling(source);
        if (nextSibling != null
            && nextSibling.IsText)
        {
          StringBuilder builder = new StringBuilder(value);
          do
          {
            builder.Append(nextSibling.Value);
            nextSibling = NextSibling(nextSibling);
          }
          while (nextSibling != null
                 && nextSibling.IsText);
          value = builder.ToString();
        }
        return value;
      }
    }

    public object UnderlyingObject
    {
      get
      {
        CalibrateText();

        return source;
      }
    }

    public override bool MoveToNamespace(string name)
    {
      if (name == document.strXmlns)
      {
        return false;
      }
      XmlElement element = source as XmlElement;
      if (element != null)
      {
        string localName;
        if (name != null
            && name.Length != 0)
        {
          localName = name;
        }
        else
        {
          localName = document.strXmlns;
        }
        string namespaceUri = document.strReservedXmlns;

        do
        {
          XmlAttribute attribute = element.GetAttributeNode(localName, namespaceUri);
          if (attribute != null)
          {
            namespaceParent = (XmlElement)source;
            source = attribute;
            return true;
          }
          element = element.ParentNode as XmlElement;
        }
        while (element != null);

        if (name == document.strXml)
        {
          namespaceParent = (XmlElement)source;
          source = document.NamespaceXml;
          return true;
        }
      }
      return false;
    }

    public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
    {
      XmlElement element = source as XmlElement;
      if (element == null)
      {
        return false;
      }
      XmlAttributeCollection attributes;
      int index = Int32.MaxValue;
      switch (scope)
      {
        case XPathNamespaceScope.Local:
          if (!element.HasAttributes)
          {
            return false;
          }
          attributes = element.Attributes;
          if (!MoveToFirstNamespaceLocal(attributes, ref index))
          {
            return false;
          }
          source = attributes[index];
          attributeIndex = index;
          namespaceParent = element;
          break;
        case XPathNamespaceScope.ExcludeXml:
          attributes = element.Attributes;
          if (!MoveToFirstNamespaceGlobal(ref attributes, ref index))
          {
            return false;
          }
          XmlAttribute attribute = attributes[index];
          while (Ref.Equal(attribute.LocalName, document.strXml))
          {
            if (!MoveToNextNamespaceGlobal(ref attributes, ref index))
            {
              return false;
            }
            attribute = attributes[index];
          }
          source = attribute;
          attributeIndex = index;
          namespaceParent = element;
          break;
        case XPathNamespaceScope.All:
          attributes = element.Attributes;
          if (!MoveToFirstNamespaceGlobal(ref attributes, ref index))
          {
            source = document.NamespaceXml;
            // attributeIndex = 0;
          }
          else
          {
            source = attributes[index];
            attributeIndex = index;
          }
          namespaceParent = element;
          break;
        default:
          return false;
      }
      return true;
    }

    private static bool MoveToFirstNamespaceLocal(XmlAttributeCollection attributes, ref int index)
    {
      for (int i = attributes.Count - 1; i >= 0; i--)
      {
        XmlAttribute attribute = attributes[i];
        if (attribute.IsNamespace)
        {
          index = i;
          return true;
        }
      }
      return false;
    }

    private static bool MoveToFirstNamespaceGlobal(ref XmlAttributeCollection attributes, ref int index)
    {
      if (MoveToFirstNamespaceLocal(attributes, ref index))
      {
        return true;
      }

      XmlElement element = attributes.parent.ParentNode as XmlElement;
      while (element != null)
      {
        if (element.HasAttributes)
        {
          attributes = element.Attributes;
          if (MoveToFirstNamespaceLocal(attributes, ref index))
          {
            return true;
          }
        }
        element = element.ParentNode as XmlElement;
      }
      return false;
    }

    public override bool MoveToNextNamespace(XPathNamespaceScope scope)
    {
      XmlAttribute attribute = source as XmlAttribute;
      if (attribute == null
          || !attribute.IsNamespace)
      {
        return false;
      }
      XmlAttributeCollection attributes;
      int index = attributeIndex;
      if (!CheckAttributePosition(attribute, out attributes, index)
          && !ResetAttributePosition(attribute, attributes, out index))
      {
        return false;
      }
      switch (scope)
      {
        case XPathNamespaceScope.Local:
          if (attribute.OwnerElement != namespaceParent)
          {
            return false;
          }
          if (!MoveToNextNamespaceLocal(attributes, ref index))
          {
            return false;
          }
          source = attributes[index];
          attributeIndex = index;
          break;
        case XPathNamespaceScope.ExcludeXml:
          string localName;
          do
          {
            if (!MoveToNextNamespaceGlobal(ref attributes, ref index))
            {
              return false;
            }
            attribute = attributes[index];
            localName = attribute.LocalName;
          }
          while (PathHasDuplicateNamespace(attribute.OwnerElement, namespaceParent, localName)
                 || Ref.Equal(localName, document.strXml));
          source = attribute;
          attributeIndex = index;
          break;
        case XPathNamespaceScope.All:
          do
          {
            if (!MoveToNextNamespaceGlobal(ref attributes, ref index))
            {
              if (PathHasDuplicateNamespace(null, namespaceParent, document.strXml))
              {
                return false;
              }
              else
              {
                source = document.NamespaceXml;
                // attributeIndex = 0;
                return true;
              }
            }
            attribute = attributes[index];
          }
          while (PathHasDuplicateNamespace(attribute.OwnerElement, namespaceParent, attribute.LocalName));
          source = attribute;
          attributeIndex = index;
          break;
        default:
          return false;
      }
      return true;
    }

    private static bool MoveToNextNamespaceLocal(XmlAttributeCollection attributes, ref int index)
    {
      for (int i = index - 1; i >= 0; i--)
      {
        XmlAttribute attribute = attributes[i];
        if (attribute.IsNamespace)
        {
          index = i;
          return true;
        }
      }
      return false;
    }

    private static bool MoveToNextNamespaceGlobal(ref XmlAttributeCollection attributes, ref int index)
    {
      if (MoveToNextNamespaceLocal(attributes, ref index))
      {
        return true;
      }

      XmlElement element = attributes.parent.ParentNode as XmlElement;
      while (element != null)
      {
        if (element.HasAttributes)
        {
          attributes = element.Attributes;
          if (MoveToFirstNamespaceLocal(attributes, ref index))
          {
            return true;
          }
        }
        element = element.ParentNode as XmlElement;
      }
      return false;
    }

    private bool PathHasDuplicateNamespace(XmlElement top, XmlElement bottom, string localName)
    {
      string namespaceUri = document.strReservedXmlns;
      while (bottom != null
             && bottom != top)
      {
        XmlAttribute attribute = bottom.GetAttributeNode(localName, namespaceUri);
        if (attribute != null)
        {
          return true;
        }
        bottom = bottom.ParentNode as XmlElement;
      }
      return false;
    }

    public override string LookupNamespace(string prefix)
    {
      string ns = base.LookupNamespace(prefix);
      if (ns != null)
      {
        ns = this.NameTable.Add(ns);
      }
      return ns;
    }

    public override bool MoveToNext()
    {
      XmlNode sibling = NextSibling(source);
      if (sibling == null)
      {
        return false;
      }
      if (sibling.IsText)
      {
        if (source.IsText)
        {
          sibling = NextSibling(TextEnd(sibling));
          if (sibling == null)
          {
            return false;
          }
        }
      }
      XmlNode parent = ParentNode(sibling);
      while (!IsValidChild(parent, sibling))
      {
        sibling = NextSibling(sibling);
        if (sibling == null)
        {
          return false;
        }
      }
      source = sibling;
      return true;
    }

    public override bool MoveToFirstChild()
    {
      XmlNode child;
      switch (source.NodeType)
      {
        case XmlNodeType.Element:
          child = FirstChild(source);
          if (child == null)
          {
            return false;
          }
          break;
        case XmlNodeType.DocumentFragment:
        case XmlNodeType.Document:
          child = FirstChild(source);
          if (child == null)
          {
            return false;
          }
          while (!IsValidChild(source, child))
          {
            child = NextSibling(child);
            if (child == null)
            {
              return false;
            }
          }
          break;
        default:
          return false;

      }
      source = child;
      return true;
    }

    public override bool MoveToParent()
    {
      XmlNode parent = ParentNode(source);
      if (parent != null)
      {
        source = parent;
        return true;
      }
      XmlAttribute attribute = source as XmlAttribute;
      if (attribute != null)
      {
        parent = attribute.IsNamespace ? namespaceParent : attribute.OwnerElement;
        if (parent != null)
        {
          source = parent;
          namespaceParent = null;
          return true;
        }
      }
      return false;
    }

    public override bool MoveTo(XPathNavigator other)
    {
      DocumentXPathNavigator that = other as DocumentXPathNavigator;
      if (that != null
          && document == that.document)
      {
        source = that.source;
        attributeIndex = that.attributeIndex;
        namespaceParent = that.namespaceParent;
        return true;
      }
      return false;
    }

    public override IXmlSchemaInfo SchemaInfo
    {
      get
      {
        return source.SchemaInfo;
      }
    }

    XmlNode IHasXmlNode.GetNode() { return source; }

    internal void ResetPosition(XmlNode node)
    {
      source = node;
      XmlAttribute attribute = node as XmlAttribute;
      if (attribute != null)
      {
        XmlElement element = attribute.OwnerElement;
        if (element != null)
        {
          ResetAttributePosition(attribute, element.Attributes, out attributeIndex);
          if (attribute.IsNamespace)
          {
            namespaceParent = element;
          }
        }
      }
    }

    private static bool ResetAttributePosition(XmlAttribute attribute, XmlAttributeCollection attributes, out int index)
    {
      if (attributes != null)
      {
        for (int i = 0; i < attributes.Count; i++)
        {
          if (attribute == attributes[i])
          {
            index = i;
            return true;
          }
        }
      }
      index = 0;
      return false;
    }

    private static bool CheckAttributePosition(XmlAttribute attribute, out XmlAttributeCollection attributes, int index)
    {
      XmlElement element = attribute.OwnerElement;
      if (element != null)
      {
        attributes = element.Attributes;
        if (index >= 0
            && index < attributes.Count
            && attribute == attributes[index])
        {
          return true;
        }
      }
      else
      {
        attributes = null;
      }
      return false;
    }

    private void CalibrateText()
    {
      XmlNode text = PreviousText(source);
      while (text != null)
      {
        ResetPosition(text);
        text = PreviousText(text);
      }
    }

    private XmlNode ParentNode(XmlNode node)
    {
      XmlNode parent = node.ParentNode;

      if (!document.HasEntityReferences)
      {
        return parent;
      }
      return ParentNodeTail(parent);
    }

    private XmlNode ParentNodeTail(XmlNode parent)
    {
      while (parent != null
             && parent.NodeType == XmlNodeType.EntityReference)
      {
        parent = parent.ParentNode;
      }
      return parent;
    }

    private XmlNode FirstChild(XmlNode node)
    {
      XmlNode child = node.FirstChild;

      if (!document.HasEntityReferences)
      {
        return child;
      }
      return FirstChildTail(child);
    }

    private XmlNode FirstChildTail(XmlNode child)
    {
      while (child != null
             && child.NodeType == XmlNodeType.EntityReference)
      {
        child = child.FirstChild;
      }
      return child;
    }

    private XmlNode NextSibling(XmlNode node)
    {
      XmlNode sibling = node.NextSibling;

      if (!document.HasEntityReferences)
      {
        return sibling;
      }
      return NextSiblingTail(node, sibling);
    }

    private XmlNode NextSiblingTail(XmlNode node, XmlNode sibling)
    {
      while (sibling == null)
      {
        node = node.ParentNode;
        if (node == null
            || node.NodeType != XmlNodeType.EntityReference)
        {
          return null;
        }
        sibling = node.NextSibling;
      }
      while (sibling != null
             && sibling.NodeType == XmlNodeType.EntityReference)
      {
        sibling = sibling.FirstChild;
      }
      return sibling;
    }

    private XmlNode PreviousText(XmlNode node)
    {
      XmlNode text = node.PreviousText;

      if (!document.HasEntityReferences)
      {
        return text;
      }
      return PreviousTextTail(node, text);
    }

    private XmlNode PreviousTextTail(XmlNode node, XmlNode text)
    {
      if (text != null)
      {
        return text;
      }
      if (!node.IsText)
      {
        return null;
      }
      XmlNode sibling = node.PreviousSibling;
      while (sibling == null)
      {
        node = node.ParentNode;
        if (node == null
            || node.NodeType != XmlNodeType.EntityReference)
        {
          return null;
        }
        sibling = node.PreviousSibling;
      }
      while (sibling != null)
      {
        switch (sibling.NodeType)
        {
          case XmlNodeType.EntityReference:
            sibling = sibling.LastChild;
            break;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
            return sibling;
          default:
            return null;
        }
      }
      return null;
    }

    private static bool IsValidChild(XmlNode parent, XmlNode child)
    {
      switch (parent.NodeType)
      {
        case XmlNodeType.Element:
          return true;
        case XmlNodeType.DocumentFragment:
          switch (child.NodeType)
          {
            case XmlNodeType.Element:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.ProcessingInstruction:
            case XmlNodeType.Comment:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
              return true;
          }
          break;
        case XmlNodeType.Document:
          switch (child.NodeType)
          {
            case XmlNodeType.Element:
            case XmlNodeType.ProcessingInstruction:
            case XmlNodeType.Comment:
              return true;
          }
          break;
        default:
          break;
      }
      return false;
    }

    private XmlNode TextEnd(XmlNode node)
    {
      XmlNode end;

      do
      {
        end = node;
        node = NextSibling(node);
      }
      while (node != null
             && node.IsText);
      return end;
    }
  }
}