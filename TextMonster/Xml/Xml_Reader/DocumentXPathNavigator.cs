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

    public override string BaseURI
    {
      get
      {
        return source.BaseURI;
      }
    }

    public override object UnderlyingObject
    {
      get
      {
        CalibrateText();

        return source;
      }
    }

    public override bool MoveToNextAttribute()
    {
      XmlAttribute attribute = source as XmlAttribute;
      if (attribute == null
          || attribute.IsNamespace)
      {
        return false;
      }
      XmlAttributeCollection attributes;
      if (!CheckAttributePosition(attribute, out attributes, attributeIndex)
          && !ResetAttributePosition(attribute, attributes, out attributeIndex))
      {
        return false;
      }
      for (int i = attributeIndex + 1; i < attributes.Count; i++)
      {
        attribute = attributes[i];
        if (!attribute.IsNamespace)
        {
          source = attribute;
          attributeIndex = i;
          return true;
        }
      }
      return false;
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

    public override void MoveToRoot()
    {
      for (; ; )
      {
        XmlNode parent = source.ParentNode;
        if (parent == null)
        {
          XmlAttribute attribute = source as XmlAttribute;
          if (attribute == null)
          {
            break;
          }
          parent = attribute.IsNamespace ? namespaceParent : attribute.OwnerElement;
          if (parent == null)
          {
            break;
          }
        }
        source = parent;
      }
      namespaceParent = null;
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

    public override bool MoveToChild(string localName, string namespaceUri)
    {
      if (source.NodeType == XmlNodeType.Attribute)
      {
        return false;
      }

      XmlNode child = FirstChild(source);
      if (child != null)
      {
        do
        {
          if (child.NodeType == XmlNodeType.Element
              && child.LocalName == localName
              && child.NamespaceURI == namespaceUri)
          {
            source = child;
            return true;
          }
          child = NextSibling(child);
        }
        while (child != null);
      }
      return false;
    }

    public override bool MoveToChild(XPathNodeType type)
    {
      if (source.NodeType == XmlNodeType.Attribute)
      {
        return false;
      }

      XmlNode child = FirstChild(source);
      if (child != null)
      {
        int mask = GetContentKindMask(type);
        if (mask == 0)
        {
          return false;
        }
        do
        {
          if (((1 << (int)child.XPNodeType) & mask) != 0)
          {
            source = child;
            return true;
          }
          child = NextSibling(child);
        }
        while (child != null);
      }
      return false;
    }

    public override bool MoveToFollowing(string localName, string namespaceUri, XPathNavigator end)
    {
      XmlNode pastFollowing = null;
      DocumentXPathNavigator that = end as DocumentXPathNavigator;
      if (that != null)
      {
        if (document != that.document)
        {
          return false;
        }
        switch (that.source.NodeType)
        {
          case XmlNodeType.Attribute:
            that = (DocumentXPathNavigator)that.Clone();
            if (!that.MoveToNonDescendant())
            {
              return false;
            }
            break;
        }
        pastFollowing = that.source;
      }

      XmlNode following = source;
      if (following.NodeType == XmlNodeType.Attribute)
      {
        following = ((XmlAttribute)following).OwnerElement;
        if (following == null)
        {
          return false;
        }
      }
      do
      {
        XmlNode firstChild = following.FirstChild;
        if (firstChild != null)
        {
          following = firstChild;
        }
        else
        {
          for (; ; )
          {
            XmlNode nextSibling = following.NextSibling;
            if (nextSibling != null)
            {
              following = nextSibling;
              break;
            }
            else
            {
              XmlNode parent = following.ParentNode;
              if (parent != null)
              {
                following = parent;
              }
              else
              {
                return false;
              }
            }
          }
        }
        if (following == pastFollowing)
        {
          return false;
        }
      }
      while (following.NodeType != XmlNodeType.Element
             || following.LocalName != localName
             || following.NamespaceURI != namespaceUri);

      source = following;
      return true;
    }

    public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
    {
      XmlNode pastFollowing = null;
      DocumentXPathNavigator that = end as DocumentXPathNavigator;
      if (that != null)
      {
        if (document != that.document)
        {
          return false;
        }
        switch (that.source.NodeType)
        {
          case XmlNodeType.Attribute:
            that = (DocumentXPathNavigator)that.Clone();
            if (!that.MoveToNonDescendant())
            {
              return false;
            }
            break;
        }
        pastFollowing = that.source;
      }

      int mask = GetContentKindMask(type);
      if (mask == 0)
      {
        return false;
      }
      XmlNode following = source;
      switch (following.NodeType)
      {
        case XmlNodeType.Attribute:
          following = ((XmlAttribute)following).OwnerElement;
          if (following == null)
          {
            return false;
          }
          break;
        case XmlNodeType.Text:
        case XmlNodeType.CDATA:
        case XmlNodeType.SignificantWhitespace:
        case XmlNodeType.Whitespace:
          following = TextEnd(following);
          break;
      }
      do
      {
        XmlNode firstChild = following.FirstChild;
        if (firstChild != null)
        {
          following = firstChild;
        }
        else
        {
          for (; ; )
          {
            XmlNode nextSibling = following.NextSibling;
            if (nextSibling != null)
            {
              following = nextSibling;
              break;
            }
            else
            {
              XmlNode parent = following.ParentNode;
              if (parent != null)
              {
                following = parent;
              }
              else
              {
                return false;
              }
            }
          }
        }
        if (following == pastFollowing)
        {
          return false;
        }
      }
      while (((1 << (int)following.XPNodeType) & mask) == 0);

      source = following;
      return true;
    }

    public override bool MoveToNext(string localName, string namespaceUri)
    {
      XmlNode sibling = NextSibling(source);
      if (sibling == null)
      {
        return false;
      }
      do
      {
        if (sibling.NodeType == XmlNodeType.Element
            && sibling.LocalName == localName
            && sibling.NamespaceURI == namespaceUri)
        {
          source = sibling;
          return true;
        }
        sibling = NextSibling(sibling);
      }
      while (sibling != null);
      return false;
    }

    public override bool MoveToNext(XPathNodeType type)
    {
      XmlNode sibling = NextSibling(source);
      if (sibling == null)
      {
        return false;
      }
      if (sibling.IsText
          && source.IsText)
      {
        sibling = NextSibling(TextEnd(sibling));
        if (sibling == null)
        {
          return false;
        }
      }

      int mask = GetContentKindMask(type);
      if (mask == 0)
      {
        return false;
      }
      do
      {
        if (((1 << (int)sibling.XPNodeType) & mask) != 0)
        {
          source = sibling;
          return true;
        }
        sibling = NextSibling(sibling);
      }
      while (sibling != null);
      return false;
    }

    public override bool IsSamePosition(XPathNavigator other)
    {
      DocumentXPathNavigator that = other as DocumentXPathNavigator;
      if (that != null)
      {
        this.CalibrateText();
        that.CalibrateText();

        return this.source == that.source
               && this.namespaceParent == that.namespaceParent;
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

    private static XmlNode OwnerNode(XmlNode node)
    {
      XmlNode parent = node.ParentNode;
      if (parent != null)
      {
        return parent;
      }
      XmlAttribute attribute = node as XmlAttribute;
      if (attribute != null)
      {
        return attribute.OwnerElement;
      }
      return null;
    }

    private static int GetDepth(XmlNode node)
    {
      int depth = 0;
      XmlNode owner = OwnerNode(node);
      while (owner != null)
      {
        depth++;
        owner = OwnerNode(owner);
      }
      return depth;
    }

    //Assuming that node1 and node2 are in the same level; Except when they are namespace nodes, they should have the same parent node
    //the returned value is node2's position corresponding to node1 
    private XmlNodeOrder Compare(XmlNode node1, XmlNode node2)
    {
      if (node1.XPNodeType == XPathNodeType.Attribute)
      {
        if (node2.XPNodeType == XPathNodeType.Attribute)
        {
          XmlElement element = ((XmlAttribute)node1).OwnerElement;
          if (element.HasAttributes)
          {
            XmlAttributeCollection attributes = element.Attributes;
            for (int i = 0; i < attributes.Count; i++)
            {
              XmlAttribute attribute = attributes[i];
              if (attribute == node1)
              {
                return XmlNodeOrder.Before;
              }
              else if (attribute == node2)
              {
                return XmlNodeOrder.After;
              }
            }
          }
          return XmlNodeOrder.Unknown;
        }
        else
        {
          return XmlNodeOrder.Before;
        }
      }
      if (node2.XPNodeType == XPathNodeType.Attribute)
      {
        return XmlNodeOrder.After;
      }

      //neither of the node is Namespace node or Attribute node
      XmlNode nextNode = node1.NextSibling;
      while (nextNode != null && nextNode != node2)
        nextNode = nextNode.NextSibling;
      if (nextNode == null)
        //didn't meet node2 in the path to the end, thus it has to be in the front of node1
        return XmlNodeOrder.After;
      else
      //met node2 in the path to the end, so node1 is at front
        return XmlNodeOrder.Before;
    }

    public override XmlNodeOrder ComparePosition(XPathNavigator other)
    {
      DocumentXPathNavigator that = other as DocumentXPathNavigator;
      if (that == null)
      {
        return XmlNodeOrder.Unknown;
      }

      this.CalibrateText();
      that.CalibrateText();

      if (this.source == that.source
          && this.namespaceParent == that.namespaceParent)
      {
        return XmlNodeOrder.Same;
      }

      if (this.namespaceParent != null
          || that.namespaceParent != null)
      {
        return base.ComparePosition(other);
      }

      XmlNode node1 = this.source;
      XmlNode node2 = that.source;

      XmlNode parent1 = OwnerNode(node1);
      XmlNode parent2 = OwnerNode(node2);
      if (parent1 == parent2)
      {
        if (parent1 == null)
        {
          return XmlNodeOrder.Unknown;
        }
        else
        {
          return Compare(node1, node2);
        }
      }

      int depth1 = GetDepth(node1);
      int depth2 = GetDepth(node2);
      if (depth2 > depth1)
      {
        while (node2 != null
               && depth2 > depth1)
        {
          node2 = OwnerNode(node2);
          depth2--;
        }
        if (node1 == node2)
        {
          return XmlNodeOrder.Before;
        }
        parent2 = OwnerNode(node2);
      }
      else if (depth1 > depth2)
      {
        while (node1 != null
               && depth1 > depth2)
        {
          node1 = OwnerNode(node1);
          depth1--;
        }
        if (node1 == node2)
        {
          return XmlNodeOrder.After;
        }
        parent1 = OwnerNode(node1);
      }

      while (parent1 != null
             && parent2 != null)
      {
        if (parent1 == parent2)
        {
          return Compare(node1, node2);
        }
        node1 = parent1;
        node2 = parent2;
        parent1 = OwnerNode(node1);
        parent2 = OwnerNode(node2);
      }
      return XmlNodeOrder.Unknown;
    }

    //the function just for XPathNodeList to enumerate current Node.
    XmlNode IHasXmlNode.GetNode() { return source; }

    public override XPathNodeIterator SelectDescendants(string localName, string namespaceURI, bool matchSelf)
    {
      string nsAtom = document.NameTable.Get(namespaceURI);
      if (nsAtom == null || this.source.NodeType == XmlNodeType.Attribute)
        return new DocumentXPathNodeIterator_Empty(this);

      string localNameAtom = document.NameTable.Get(localName);
      if (localNameAtom == null)
        return new DocumentXPathNodeIterator_Empty(this);

      if (localNameAtom.Length == 0)
      {
        if (matchSelf)
          return new DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName(this, nsAtom);
        return new DocumentXPathNodeIterator_ElemChildren_NoLocalName(this, nsAtom);
      }

      if (matchSelf)
        return new DocumentXPathNodeIterator_ElemChildren_AndSelf(this, localNameAtom, nsAtom);
      return new DocumentXPathNodeIterator_ElemChildren(this, localNameAtom, nsAtom);
    }

    public override XPathNodeIterator SelectDescendants(XPathNodeType nt, bool includeSelf)
    {
      if (nt == XPathNodeType.Element)
      {
        XmlNodeType curNT = source.NodeType;
        if (curNT != XmlNodeType.Document && curNT != XmlNodeType.Element)
        {
          //only Document, Entity, Element node can have Element node as children ( descendant )
          //entity nodes should be invisible to XPath data model
          return new DocumentXPathNodeIterator_Empty(this);
        }
        if (includeSelf)
          return new DocumentXPathNodeIterator_AllElemChildren_AndSelf(this);
        return new DocumentXPathNodeIterator_AllElemChildren(this);
      }
      return base.SelectDescendants(nt, includeSelf);
    }

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

    private XmlNode PreviousSibling(XmlNode node)
    {
      XmlNode sibling = node.PreviousSibling;

      if (!document.HasEntityReferences)
      {
        return sibling;
      }
      return PreviousSiblingTail(node, sibling);
    }

    private XmlNode PreviousSiblingTail(XmlNode node, XmlNode sibling)
    {
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
      while (sibling != null
             && sibling.NodeType == XmlNodeType.EntityReference)
      {
        sibling = sibling.LastChild;
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

    private XmlNode TextStart(XmlNode node)
    {
      XmlNode start;

      do
      {
        start = node;
        node = PreviousSibling(node);
      }
      while (node != null
             && node.IsText);
      return start;
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