using System;
using System.Xml;

namespace TextMonster.Xml
{
  internal class XNodeReader : XmlReader, IXmlLineInfo
  {
    object source;
    object parent;
    ReadState state;
    XNode root;
    readonly XmlNameTable nameTable;
    readonly bool omitDuplicateNamespaces;

    public override int AttributeCount
    {
      get
      {
        if (!IsInteractive)
          return 0;
        int num = 0;
        var inAttributeScope = GetElementInAttributeScope();
        if (inAttributeScope != null)
        {
          var candidateAttribute = inAttributeScope.lastAttr;
          if (candidateAttribute != null)
          {
            do
            {
              candidateAttribute = candidateAttribute.next;
              if (!omitDuplicateNamespaces || !IsDuplicateNamespaceAttribute(candidateAttribute))
                ++num;
            }
            while (candidateAttribute != inAttributeScope.lastAttr);
          }
        }
        return num;
      }
    }

    public override string BaseURI
    {
      get
      {
        var xobject1 = source as XObject;
        if (xobject1 != null)
          return xobject1.BaseUri;
        var xobject2 = parent as XObject;
        if (xobject2 != null)
          return xobject2.BaseUri;
        return string.Empty;
      }
    }

    public override int Depth
    {
      get
      {
        if (!IsInteractive)
          return 0;
        var o1 = source as XObject;
        if (o1 != null)
          return GetDepth(o1);
        var o2 = parent as XObject;
        if (o2 != null)
          return GetDepth(o2) + 1;
        return 0;
      }
    }

    public override bool EOF
    {
      get
      {
        return state == ReadState.EndOfFile;
      }
    }

    public override bool HasAttributes
    {
      get
      {
        if (!IsInteractive)
          return false;
        var inAttributeScope = GetElementInAttributeScope();
        if (inAttributeScope == null || inAttributeScope.lastAttr == null)
          return false;
        if (omitDuplicateNamespaces)
          return GetFirstNonDuplicateNamespaceAttribute(inAttributeScope.lastAttr.next) != null;
        return true;
      }
    }

    public override bool HasValue
    {
      get
      {
        if (!IsInteractive)
          return false;
        var xobject = source as XObject;
        if (xobject == null)
          return true;
        switch (xobject.NodeType)
        {
          case XmlNodeType.Attribute:
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.DocumentType:
          return true;
          default:
          return false;
        }
      }
    }

    public override bool IsEmptyElement
    {
      get
      {
        if (!IsInteractive)
          return false;
        var xelement = source as XElement;
        if (xelement != null)
          return xelement.IsEmpty;
        return false;
      }
    }

    public override string LocalName
    {
      get
      {
        return nameTable.Add(GetLocalName());
      }
    }

    public override string Name
    {
      get
      {
        string prefix = GetPrefix();
        if (prefix.Length == 0)
          return nameTable.Add(GetLocalName());
        return nameTable.Add(prefix + ":" + GetLocalName());
      }
    }

    public override string NamespaceURI
    {
      get
      {
        return nameTable.Add(GetNamespaceUri());
      }
    }

    public override XmlNameTable NameTable
    {
      get
      {
        return nameTable;
      }
    }

    public override XmlNodeType NodeType
    {
      get
      {
        if (!IsInteractive)
          return XmlNodeType.None;
        var xobject = source as XObject;
        if (xobject != null)
        {
          if (IsEndElement)
            return XmlNodeType.EndElement;
          var nodeType = xobject.NodeType;
          if (nodeType != XmlNodeType.Text)
            return nodeType;
          return xobject.parent != null && xobject.parent.parent == null && xobject.parent is XDocument ? XmlNodeType.Whitespace : XmlNodeType.Text;
        }
        return parent is XDocument ? XmlNodeType.Whitespace : XmlNodeType.Text;
      }
    }

    public override string Prefix
    {
      get
      {
        return nameTable.Add(GetPrefix());
      }
    }

    public override ReadState ReadState
    {
      get
      {
        return state;
      }
    }

    public override XmlReaderSettings Settings
    {
      get
      {
        return new XmlReaderSettings
        {
          CheckCharacters = false
        };
      }
    }

    public override string Value
    {
      get
      {
        if (!IsInteractive)
          return string.Empty;
        var xobject = source as XObject;
        if (xobject == null)
          return (string)source;
        switch (xobject.NodeType)
        {
          case XmlNodeType.Attribute:
          return ((XAttribute)xobject).Value;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          return ((XText)xobject).Value;
          case XmlNodeType.ProcessingInstruction:
          return ((XProcessingInstruction)xobject).Data;
          case XmlNodeType.Comment:
          return ((XComment)xobject).Value;
          case XmlNodeType.DocumentType:
          return ((XDocumentType)xobject).InternalSubset;
          default:
          return string.Empty;
        }
      }
    }

    public override string XmlLang
    {
      get
      {
        if (!IsInteractive)
          return string.Empty;
        var xelement = GetElementInScope();
        if (xelement != null)
        {
          var name = XNamespace.Xml.GetName("lang");
          do
          {
            var xattribute = xelement.Attribute(name);
            if (xattribute != null)
              return xattribute.Value;
            xelement = xelement.parent as XElement;
          }
          while (xelement != null);
        }
        return string.Empty;
      }
    }

    public override XmlSpace XmlSpace
    {
      get
      {
        if (!IsInteractive)
          return XmlSpace.None;
        var xelement = GetElementInScope();
        if (xelement != null)
        {
          var name = XNamespace.Xml.GetName("space");
          do
          {
            var xattribute = xelement.Attribute(name);
            if (xattribute != null)
            {
              string str = xattribute.Value.Trim(' ', '\t', '\n', '\r');
              if (str == "preserve")
                return XmlSpace.Preserve;
              if (str == "default")
                return XmlSpace.Default;
            }
            xelement = xelement.parent as XElement;
          }
          while (xelement != null);
        }
        return XmlSpace.None;
      }
    }

    int IXmlLineInfo.LineNumber
    {
      get
      {
        if (IsEndElement)
        {
          var xelement = source as XElement;
          if (xelement != null)
          {
            var elementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
            if (elementAnnotation != null)
              return elementAnnotation.lineNumber;
          }
        }
        else
        {
          var xmlLineInfo = source as IXmlLineInfo;
          if (xmlLineInfo != null)
            return xmlLineInfo.LineNumber;
        }
        return 0;
      }
    }

    int IXmlLineInfo.LinePosition
    {
      get
      {
        if (IsEndElement)
        {
          var xelement = source as XElement;
          if (xelement != null)
          {
            var elementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
            if (elementAnnotation != null)
              return elementAnnotation.linePosition;
          }
        }
        else
        {
          var xmlLineInfo = source as IXmlLineInfo;
          if (xmlLineInfo != null)
            return xmlLineInfo.LinePosition;
        }
        return 0;
      }
    }

    bool IsEndElement
    {
      get
      {
        return parent == source;
      }
      set
      {
        parent = value ? source : null;
      }
    }

    bool IsInteractive
    {
      get
      {
        return state == ReadState.Interactive;
      }
    }

    internal XNodeReader(XNode node, XmlNameTable nameTable, ReaderOptions options)
    {
      source = node;
      root = node;
      this.nameTable = nameTable ?? CreateNameTable();
      omitDuplicateNamespaces = (options & ReaderOptions.OmitDuplicateNamespaces) != ReaderOptions.None;
    }

    internal XNodeReader(XNode node, XmlNameTable nameTable)
      : this(node, nameTable, (node.GetSaveOptionsFromAnnotations() & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None ? ReaderOptions.OmitDuplicateNamespaces : ReaderOptions.None)
    {
    }

    static int GetDepth(XObject o)
    {
      int num = 0;
      for (; o.parent != null; o = (XObject)o.parent)
        ++num;
      if (o is XDocument)
        --num;
      return num;
    }

    string GetLocalName()
    {
      if (!IsInteractive)
        return string.Empty;
      var xelement = source as XElement;
      if (xelement != null)
        return xelement.Name.LocalName;
      var xattribute = source as XAttribute;
      if (xattribute != null)
        return xattribute.Name.LocalName;
      var xprocessingInstruction = source as XProcessingInstruction;
      if (xprocessingInstruction != null)
        return xprocessingInstruction.Target;
      var xdocumentType = source as XDocumentType;
      if (xdocumentType != null)
        return xdocumentType.Name;
      return string.Empty;
    }

    string GetNamespaceUri()
    {
      if (!IsInteractive)
        return string.Empty;
      var xelement = source as XElement;
      if (xelement != null)
        return xelement.Name.NamespaceName;
      var xattribute = source as XAttribute;
      if (xattribute == null)
        return string.Empty;
      string namespaceName = xattribute.Name.NamespaceName;
      if (namespaceName.Length == 0 && xattribute.Name.LocalName == "xmlns")
        return "http://www.w3.org/2000/xmlns/";
      return namespaceName;
    }

    string GetPrefix()
    {
      if (!IsInteractive)
        return string.Empty;
      var xelement = source as XElement;
      if (xelement != null)
        return xelement.GetPrefixOfNamespace(xelement.Name.Namespace) ?? string.Empty;
      var xattribute = source as XAttribute;
      if (xattribute != null)
      {
        string prefixOfNamespace = xattribute.GetPrefixOfNamespace(xattribute.Name.Namespace);
        if (prefixOfNamespace != null)
          return prefixOfNamespace;
      }
      return string.Empty;
    }

    public override void Close()
    {
      source = null;
      parent = null;
      root = null;
      state = ReadState.Closed;
    }

    public override string GetAttribute(string name)
    {
      if (!IsInteractive)
        return null;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        string localName;
        string namespaceName;
        GetNameInAttributeScope(name, inAttributeScope, out localName, out namespaceName);
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (omitDuplicateNamespaces && IsDuplicateNamespaceAttribute(candidateAttribute))
                return null;
              return candidateAttribute.Value;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
        return null;
      }
      var xdocumentType = source as XDocumentType;
      if (xdocumentType != null)
      {
        if (name == "PUBLIC")
          return xdocumentType.PublicId;
        if (name == "SYSTEM")
          return xdocumentType.SystemId;
      }
      return null;
    }

    public override string GetAttribute(string localName, string namespaceName)
    {
      if (!IsInteractive)
        return null;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        if (localName == "xmlns")
        {
          if (namespaceName.Length == 0)
            return null;
          if (namespaceName == "http://www.w3.org/2000/xmlns/")
            namespaceName = string.Empty;
        }
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (omitDuplicateNamespaces && IsDuplicateNamespaceAttribute(candidateAttribute))
                return null;
              return candidateAttribute.Value;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return null;
    }

    public override string GetAttribute(int index)
    {
      if (!IsInteractive)
        return null;
      if (index < 0)
        return null;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if ((!omitDuplicateNamespaces || !IsDuplicateNamespaceAttribute(candidateAttribute)) && index-- == 0)
              return candidateAttribute.Value;
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return null;
    }

    public override string LookupNamespace(string prefix)
    {
      if (!IsInteractive)
        return null;
      var elementInScope = GetElementInScope();
      if (elementInScope != null)
      {
        var xnamespace = prefix.Length == 0 ? elementInScope.GetDefaultNamespace() : elementInScope.GetNamespaceOfPrefix(prefix);
        if (xnamespace != null)
          return nameTable.Add(xnamespace.NamespaceName);
      }
      return null;
    }

    public override bool MoveToAttribute(string name)
    {
      if (!IsInteractive)
        return false;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        string localName;
        string namespaceName;
        GetNameInAttributeScope(name, inAttributeScope, out localName, out namespaceName);
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (omitDuplicateNamespaces && IsDuplicateNamespaceAttribute(candidateAttribute))
                return false;
              source = candidateAttribute;
              parent = null;
              return true;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return false;
    }

    public override bool MoveToAttribute(string localName, string namespaceName)
    {
      if (!IsInteractive)
        return false;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        if (localName == "xmlns")
        {
          if (namespaceName.Length == 0)
            return false;
          if (namespaceName == "http://www.w3.org/2000/xmlns/")
            namespaceName = string.Empty;
        }
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (omitDuplicateNamespaces && IsDuplicateNamespaceAttribute(candidateAttribute))
                return false;
              source = candidateAttribute;
              parent = null;
              return true;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return false;
    }

    public override void MoveToAttribute(int index)
    {
      if (!IsInteractive)
        return;
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        var candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if ((!omitDuplicateNamespaces || !IsDuplicateNamespaceAttribute(candidateAttribute)) && index-- == 0)
            {
              source = candidateAttribute;
              parent = null;
              return;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      throw new ArgumentOutOfRangeException("index");
    }

    public override bool MoveToElement()
    {
      if (!IsInteractive)
        return false;
      var xattribute = source as XAttribute ?? parent as XAttribute;
      if (xattribute == null || xattribute.parent == null)
        return false;
      source = xattribute.parent;
      parent = null;
      return true;
    }

    public override bool MoveToFirstAttribute()
    {
      if (!IsInteractive)
        return false;
      var inAttributeScope = GetElementInAttributeScope();
      if (inAttributeScope == null || inAttributeScope.lastAttr == null)
        return false;
      if (omitDuplicateNamespaces)
      {
        var obj = GetFirstNonDuplicateNamespaceAttribute(inAttributeScope.lastAttr.next);
        if (obj == null)
          return false;
        source = obj;
      }
      else
        source = inAttributeScope.lastAttr.next;
      return true;
    }

    public override bool MoveToNextAttribute()
    {
      if (!IsInteractive)
        return false;
      var xelement = source as XElement;
      if (xelement != null)
      {
        if (IsEndElement || xelement.lastAttr == null)
          return false;
        if (omitDuplicateNamespaces)
        {
          var obj = GetFirstNonDuplicateNamespaceAttribute(xelement.lastAttr.next);
          if (obj == null)
            return false;
          source = obj;
        }
        else
          source = xelement.lastAttr.next;
        return true;
      }
      var xattribute = source as XAttribute ?? parent as XAttribute;
      if (xattribute == null || xattribute.parent == null || ((XElement)xattribute.parent).lastAttr == xattribute)
        return false;
      if (omitDuplicateNamespaces)
      {
        var obj = GetFirstNonDuplicateNamespaceAttribute(xattribute.next);
        if (obj == null)
          return false;
        source = obj;
      }
      else
        source = xattribute.next;
      parent = null;
      return true;
    }

    public override bool Read()
    {
      switch (state)
      {
        case ReadState.Initial:
        state = ReadState.Interactive;
        var d = source as XDocument;
        if (d != null)
          return ReadIntoDocument(d);
        return true;
        case ReadState.Interactive:
        return Read(false);
        default:
        return false;
      }
    }

    public override bool ReadAttributeValue()
    {
      if (!IsInteractive)
        return false;
      var a = source as XAttribute;
      if (a != null)
        return ReadIntoAttribute(a);
      return false;
    }

    public override bool ReadToDescendant(string localName, string namespaceName)
    {
      if (!IsInteractive)
        return false;
      MoveToElement();
      var xelement1 = source as XElement;
      if (xelement1 != null && !xelement1.IsEmpty && !IsEndElement)
      {
        foreach (var xelement2 in xelement1.Descendants())
        {
          if (xelement2.Name.LocalName == localName && xelement2.Name.NamespaceName == namespaceName)
          {
            source = xelement2;
            return true;
          }
        }
        IsEndElement = true;
      }
      return false;
    }

    public override bool ReadToFollowing(string localName, string namespaceName)
    {
      while (Read())
      {
        var xelement = source as XElement;
        if (xelement != null && !IsEndElement && (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName))
          return true;
      }
      return false;
    }

    public override bool ReadToNextSibling(string localName, string namespaceName)
    {
      if (!IsInteractive)
        return false;
      MoveToElement();
      if (source != root)
      {
        var xnode = source as XNode;
        if (xnode != null)
        {
          foreach (var xelement in xnode.ElementsAfterSelf())
          {
            if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
            {
              source = xelement;
              IsEndElement = false;
              return true;
            }
          }
          if (xnode.parent is XElement)
          {
            source = xnode.parent;
            IsEndElement = true;
            return false;
          }
        }
        else if (parent is XElement)
        {
          source = parent;
          parent = null;
          IsEndElement = true;
          return false;
        }
      }
      return ReadToEnd();
    }

    public override void ResolveEntity()
    {
    }

    public override void Skip()
    {
      if (!IsInteractive)
        return;
      Read(true);
    }

    bool IXmlLineInfo.HasLineInfo()
    {
      if (IsEndElement)
      {
        var xelement = source as XElement;
        if (xelement != null)
          return xelement.Annotation<LineInfoEndElementAnnotation>() != null;
      }
      else
      {
        var xmlLineInfo = source as IXmlLineInfo;
        if (xmlLineInfo != null)
          return xmlLineInfo.HasLineInfo();
      }
      return false;
    }

    static XmlNameTable CreateNameTable()
    {
      var xmlNameTable = (XmlNameTable)new NameTable();
      xmlNameTable.Add(string.Empty);
      xmlNameTable.Add("http://www.w3.org/2000/xmlns/");
      xmlNameTable.Add("http://www.w3.org/XML/1998/namespace");
      return xmlNameTable;
    }

    XElement GetElementInAttributeScope()
    {
      var xelement = source as XElement;
      if (xelement != null)
      {
        if (IsEndElement)
          return null;
        return xelement;
      }
      var xattribute1 = source as XAttribute;
      if (xattribute1 != null)
        return (XElement)xattribute1.parent;
      var xattribute2 = parent as XAttribute;
      if (xattribute2 != null)
        return (XElement)xattribute2.parent;
      return null;
    }

    XElement GetElementInScope()
    {
      var xelement1 = source as XElement;
      if (xelement1 != null)
        return xelement1;
      var xnode = source as XNode;
      if (xnode != null)
        return xnode.parent as XElement;
      var xattribute1 = source as XAttribute;
      if (xattribute1 != null)
        return (XElement)xattribute1.parent;
      var xelement2 = parent as XElement;
      if (xelement2 != null)
        return xelement2;
      var xattribute2 = parent as XAttribute;
      if (xattribute2 != null)
        return (XElement)xattribute2.parent;
      return null;
    }

    static void GetNameInAttributeScope(string qualifiedName, XElement e, out string localName, out string namespaceName)
    {
      if (!string.IsNullOrEmpty(qualifiedName))
      {
        int length = qualifiedName.IndexOf(':');
        if (length != 0 && length != qualifiedName.Length - 1)
        {
          if (length == -1)
          {
            localName = qualifiedName;
            namespaceName = string.Empty;
            return;
          }
          var namespaceOfPrefix = e.GetNamespaceOfPrefix(qualifiedName.Substring(0, length));
          if (namespaceOfPrefix != null)
          {
            localName = qualifiedName.Substring(length + 1, qualifiedName.Length - length - 1);
            namespaceName = namespaceOfPrefix.NamespaceName;
            return;
          }
        }
      }
      localName = null;
      namespaceName = null;
    }

    bool Read(bool skipContent)
    {
      var e = source as XElement;
      if (e != null)
      {
        if (((e.IsEmpty ? 1 : (IsEndElement ? 1 : 0)) | (skipContent ? 1 : 0)) != 0)
          return ReadOverNode(e);
        return ReadIntoElement(e);
      }
      var n = source as XNode;
      if (n != null)
        return ReadOverNode(n);
      var a = source as XAttribute;
      if (a != null)
        return ReadOverAttribute(a, skipContent);
      return ReadOverText(skipContent);
    }

    bool ReadIntoDocument(XDocument d)
    {
      var xnode = d.content as XNode;
      if (xnode != null)
      {
        source = xnode.next;
        return true;
      }
      string str = d.content as string;
      if (str == null || str.Length <= 0)
        return ReadToEnd();
      source = str;
      parent = d;
      return true;
    }

    bool ReadIntoElement(XElement e)
    {
      var xnode = e.content as XNode;
      if (xnode != null)
      {
        source = xnode.next;
        return true;
      }
      string str = e.content as string;
      if (str == null)
        return ReadToEnd();
      if (str.Length > 0)
      {
        source = str;
        parent = e;
      }
      else
      {
        source = e;
        IsEndElement = true;
      }
      return true;
    }

    bool ReadIntoAttribute(XAttribute a)
    {
      source = a.value;
      parent = a;
      return true;
    }

    bool ReadOverAttribute(XAttribute a, bool skipContent)
    {
      var e = (XElement)a.parent;
      if (e == null)
        return ReadToEnd();
      if (e.IsEmpty | skipContent)
        return ReadOverNode(e);
      return ReadIntoElement(e);
    }

    bool ReadOverNode(XNode n)
    {
      if (n == root)
        return ReadToEnd();
      var xnode = n.next;
      if (xnode == null || xnode == n || n == n.parent.content)
      {
        if (n.parent == null || n.parent.parent == null && n.parent is XDocument)
          return ReadToEnd();
        source = n.parent;
        IsEndElement = true;
      }
      else
      {
        source = xnode;
        IsEndElement = false;
      }
      return true;
    }

    bool ReadOverText(bool skipContent)
    {
      if (parent is XElement)
      {
        source = parent;
        parent = null;
        IsEndElement = true;
        return true;
      }
      if (!(parent is XAttribute))
        return ReadToEnd();
      var a = (XAttribute)parent;
      parent = null;
      return ReadOverAttribute(a, skipContent);
    }

    bool ReadToEnd()
    {
      state = ReadState.EndOfFile;
      return false;
    }

    bool IsDuplicateNamespaceAttribute(XAttribute candidateAttribute)
    {
      if (!candidateAttribute.IsNamespaceDeclaration)
        return false;
      return IsDuplicateNamespaceAttributeInner(candidateAttribute);
    }

    bool IsDuplicateNamespaceAttributeInner(XAttribute candidateAttribute)
    {
      if (candidateAttribute.Name.LocalName == "xml")
        return true;
      var xelement1 = candidateAttribute.parent as XElement;
      if (xelement1 == root || xelement1 == null)
        return false;
      for (var xelement2 = xelement1.parent as XElement; xelement2 != null; xelement2 = xelement2.parent as XElement)
      {
        var xattribute = xelement2.lastAttr;
        if (xattribute != null)
        {
          while (!(xattribute.name == candidateAttribute.name))
          {
            xattribute = xattribute.next;
            if (xattribute == xelement2.lastAttr)
              goto label_11;
          }
          return xattribute.Value == candidateAttribute.Value;
        }
      label_11:
        if (xelement2 == root)
          return false;
      }
      return false;
    }

    XAttribute GetFirstNonDuplicateNamespaceAttribute(XAttribute candidate)
    {
      if (!IsDuplicateNamespaceAttribute(candidate))
        return candidate;
      var xelement = candidate.parent as XElement;
      if (xelement != null && candidate != xelement.lastAttr)
      {
        do
        {
          candidate = candidate.next;
          if (!IsDuplicateNamespaceAttribute(candidate))
            return candidate;
        }
        while (candidate != xelement.lastAttr);
      }
      return null;
    }
  }
}
