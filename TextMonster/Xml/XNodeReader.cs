using System;
using System.Xml;

namespace TextMonster.Xml
{
  internal class XNodeReader : XmlReader, IXmlLineInfo
  {
    private object source;
    private object parent;
    private ReadState state;
    private XNode root;
    private XmlNameTable nameTable;
    private bool omitDuplicateNamespaces;
    private IDtdInfo dtdInfo;
    private bool dtdInfoInitialized;

    public override int AttributeCount
    {
      get
      {
        if (!this.IsInteractive)
          return 0;
        int num = 0;
        XElement inAttributeScope = this.GetElementInAttributeScope();
        if (inAttributeScope != null)
        {
          XAttribute candidateAttribute = inAttributeScope.lastAttr;
          if (candidateAttribute != null)
          {
            do
            {
              candidateAttribute = candidateAttribute.next;
              if (!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(candidateAttribute))
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
        XObject xobject1 = this.source as XObject;
        if (xobject1 != null)
          return xobject1.BaseUri;
        XObject xobject2 = this.parent as XObject;
        if (xobject2 != null)
          return xobject2.BaseUri;
        return string.Empty;
      }
    }

    public override int Depth
    {
      get
      {
        if (!this.IsInteractive)
          return 0;
        XObject o1 = this.source as XObject;
        if (o1 != null)
          return XNodeReader.GetDepth(o1);
        XObject o2 = this.parent as XObject;
        if (o2 != null)
          return XNodeReader.GetDepth(o2) + 1;
        return 0;
      }
    }

    public override bool EOF
    {
      get
      {
        return this.state == ReadState.EndOfFile;
      }
    }

    public override bool HasAttributes
    {
      get
      {
        if (!this.IsInteractive)
          return false;
        XElement inAttributeScope = this.GetElementInAttributeScope();
        if (inAttributeScope == null || inAttributeScope.lastAttr == null)
          return false;
        if (this.omitDuplicateNamespaces)
          return this.GetFirstNonDuplicateNamespaceAttribute(inAttributeScope.lastAttr.next) != null;
        return true;
      }
    }

    public override bool HasValue
    {
      get
      {
        if (!this.IsInteractive)
          return false;
        XObject xobject = this.source as XObject;
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
        if (!this.IsInteractive)
          return false;
        XElement xelement = this.source as XElement;
        if (xelement != null)
          return xelement.IsEmpty;
        return false;
      }
    }

    public override string LocalName
    {
      get
      {
        return this.nameTable.Add(this.GetLocalName());
      }
    }

    public override string Name
    {
      get
      {
        string prefix = this.GetPrefix();
        if (prefix.Length == 0)
          return this.nameTable.Add(this.GetLocalName());
        return this.nameTable.Add(prefix + ":" + this.GetLocalName());
      }
    }

    public override string NamespaceURI
    {
      get
      {
        return this.nameTable.Add(this.GetNamespaceURI());
      }
    }

    public override XmlNameTable NameTable
    {
      get
      {
        return this.nameTable;
      }
    }

    public override XmlNodeType NodeType
    {
      get
      {
        if (!this.IsInteractive)
          return XmlNodeType.None;
        XObject xobject = this.source as XObject;
        if (xobject != null)
        {
          if (this.IsEndElement)
            return XmlNodeType.EndElement;
          XmlNodeType nodeType = xobject.NodeType;
          if (nodeType != XmlNodeType.Text)
            return nodeType;
          return xobject.parent != null && xobject.parent.parent == null && xobject.parent is XDocument ? XmlNodeType.Whitespace : XmlNodeType.Text;
        }
        return this.parent is XDocument ? XmlNodeType.Whitespace : XmlNodeType.Text;
      }
    }

    public override string Prefix
    {
      get
      {
        return this.nameTable.Add(this.GetPrefix());
      }
    }

    public override ReadState ReadState
    {
      get
      {
        return this.state;
      }
    }

    public override XmlReaderSettings Settings
    {
      get
      {
        return new XmlReaderSettings()
        {
          CheckCharacters = false
        };
      }
    }

    public override string Value
    {
      get
      {
        if (!this.IsInteractive)
          return string.Empty;
        XObject xobject = this.source as XObject;
        if (xobject == null)
          return (string)this.source;
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
        if (!this.IsInteractive)
          return string.Empty;
        XElement xelement = this.GetElementInScope();
        if (xelement != null)
        {
          XName name = XNamespace.Xml.GetName("lang");
          do
          {
            XAttribute xattribute = xelement.Attribute(name);
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
        if (!this.IsInteractive)
          return XmlSpace.None;
        XElement xelement = this.GetElementInScope();
        if (xelement != null)
        {
          XName name = XNamespace.Xml.GetName("space");
          do
          {
            XAttribute xattribute = xelement.Attribute(name);
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

    internal override IDtdInfo DtdInfo
    {
      get
      {
        if (this.dtdInfoInitialized)
          return this.dtdInfo;
        this.dtdInfoInitialized = true;
        XDocumentType xdocumentType = this.source as XDocumentType;
        if (xdocumentType == null)
        {
          for (XNode xnode = this.root; xnode != null; xnode = (XNode)xnode.parent)
          {
            XDocument xdocument = xnode as XDocument;
            if (xdocument != null)
            {
              xdocumentType = xdocument.DocumentType;
              break;
            }
          }
        }
        if (xdocumentType != null)
          this.dtdInfo = xdocumentType.DtdInfo;
        return this.dtdInfo;
      }
    }

    int IXmlLineInfo.LineNumber
    {
      get
      {
        if (this.IsEndElement)
        {
          XElement xelement = this.source as XElement;
          if (xelement != null)
          {
            LineInfoEndElementAnnotation elementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
            if (elementAnnotation != null)
              return elementAnnotation.lineNumber;
          }
        }
        else
        {
          IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
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
        if (this.IsEndElement)
        {
          XElement xelement = this.source as XElement;
          if (xelement != null)
          {
            LineInfoEndElementAnnotation elementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
            if (elementAnnotation != null)
              return elementAnnotation.linePosition;
          }
        }
        else
        {
          IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
          if (xmlLineInfo != null)
            return xmlLineInfo.LinePosition;
        }
        return 0;
      }
    }

    private bool IsEndElement
    {
      get
      {
        return this.parent == this.source;
      }
      set
      {
        this.parent = value ? this.source : (object)null;
      }
    }

    private bool IsInteractive
    {
      get
      {
        return this.state == ReadState.Interactive;
      }
    }

    internal XNodeReader(XNode node, XmlNameTable nameTable, ReaderOptions options)
    {
      this.source = (object)node;
      this.root = node;
      this.nameTable = nameTable != null ? nameTable : XNodeReader.CreateNameTable();
      this.omitDuplicateNamespaces = (options & ReaderOptions.OmitDuplicateNamespaces) != ReaderOptions.None;
    }

    internal XNodeReader(XNode node, XmlNameTable nameTable)
      : this(node, nameTable, (node.GetSaveOptionsFromAnnotations() & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None ? ReaderOptions.OmitDuplicateNamespaces : ReaderOptions.None)
    {
    }

    private static int GetDepth(XObject o)
    {
      int num = 0;
      for (; o.parent != null; o = (XObject)o.parent)
        ++num;
      if (o is XDocument)
        --num;
      return num;
    }

    private string GetLocalName()
    {
      if (!this.IsInteractive)
        return string.Empty;
      XElement xelement = this.source as XElement;
      if (xelement != null)
        return xelement.Name.LocalName;
      XAttribute xattribute = this.source as XAttribute;
      if (xattribute != null)
        return xattribute.Name.LocalName;
      XProcessingInstruction xprocessingInstruction = this.source as XProcessingInstruction;
      if (xprocessingInstruction != null)
        return xprocessingInstruction.Target;
      XDocumentType xdocumentType = this.source as XDocumentType;
      if (xdocumentType != null)
        return xdocumentType.Name;
      return string.Empty;
    }

    private string GetNamespaceURI()
    {
      if (!this.IsInteractive)
        return string.Empty;
      XElement xelement = this.source as XElement;
      if (xelement != null)
        return xelement.Name.NamespaceName;
      XAttribute xattribute = this.source as XAttribute;
      if (xattribute == null)
        return string.Empty;
      string namespaceName = xattribute.Name.NamespaceName;
      if (namespaceName.Length == 0 && xattribute.Name.LocalName == "xmlns")
        return "http://www.w3.org/2000/xmlns/";
      return namespaceName;
    }

    private string GetPrefix()
    {
      if (!this.IsInteractive)
        return string.Empty;
      XElement xelement = this.source as XElement;
      if (xelement != null)
        return xelement.GetPrefixOfNamespace(xelement.Name.Namespace) ?? string.Empty;
      XAttribute xattribute = this.source as XAttribute;
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
      this.source = (object)null;
      this.parent = (object)null;
      this.root = (XNode)null;
      this.state = ReadState.Closed;
    }

    public override string GetAttribute(string name)
    {
      if (!this.IsInteractive)
        return (string)null;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        string localName;
        string namespaceName;
        XNodeReader.GetNameInAttributeScope(name, inAttributeScope, out localName, out namespaceName);
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(candidateAttribute))
                return (string)null;
              return candidateAttribute.Value;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
        return (string)null;
      }
      XDocumentType xdocumentType = this.source as XDocumentType;
      if (xdocumentType != null)
      {
        if (name == "PUBLIC")
          return xdocumentType.PublicId;
        if (name == "SYSTEM")
          return xdocumentType.SystemId;
      }
      return (string)null;
    }

    public override string GetAttribute(string localName, string namespaceName)
    {
      if (!this.IsInteractive)
        return (string)null;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        if (localName == "xmlns")
        {
          if (namespaceName != null && namespaceName.Length == 0)
            return (string)null;
          if (namespaceName == "http://www.w3.org/2000/xmlns/")
            namespaceName = string.Empty;
        }
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(candidateAttribute))
                return (string)null;
              return candidateAttribute.Value;
            }
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return (string)null;
    }

    public override string GetAttribute(int index)
    {
      if (!this.IsInteractive)
        return (string)null;
      if (index < 0)
        return (string)null;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if ((!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(candidateAttribute)) && index-- == 0)
              return candidateAttribute.Value;
          }
          while (candidateAttribute != inAttributeScope.lastAttr);
        }
      }
      return (string)null;
    }

    public override string LookupNamespace(string prefix)
    {
      if (!this.IsInteractive)
        return (string)null;
      if (prefix == null)
        return (string)null;
      XElement elementInScope = this.GetElementInScope();
      if (elementInScope != null)
      {
        XNamespace xnamespace = prefix.Length == 0 ? elementInScope.GetDefaultNamespace() : elementInScope.GetNamespaceOfPrefix(prefix);
        if (xnamespace != (XNamespace)null)
          return this.nameTable.Add(xnamespace.NamespaceName);
      }
      return (string)null;
    }

    public override bool MoveToAttribute(string name)
    {
      if (!this.IsInteractive)
        return false;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        string localName;
        string namespaceName;
        XNodeReader.GetNameInAttributeScope(name, inAttributeScope, out localName, out namespaceName);
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(candidateAttribute))
                return false;
              this.source = (object)candidateAttribute;
              this.parent = (object)null;
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
      if (!this.IsInteractive)
        return false;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        if (localName == "xmlns")
        {
          if (namespaceName != null && namespaceName.Length == 0)
            return false;
          if (namespaceName == "http://www.w3.org/2000/xmlns/")
            namespaceName = string.Empty;
        }
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if (candidateAttribute.Name.LocalName == localName && candidateAttribute.Name.NamespaceName == namespaceName)
            {
              if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(candidateAttribute))
                return false;
              this.source = (object)candidateAttribute;
              this.parent = (object)null;
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
      if (!this.IsInteractive)
        return;
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope != null)
      {
        XAttribute candidateAttribute = inAttributeScope.lastAttr;
        if (candidateAttribute != null)
        {
          do
          {
            candidateAttribute = candidateAttribute.next;
            if ((!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(candidateAttribute)) && index-- == 0)
            {
              this.source = (object)candidateAttribute;
              this.parent = (object)null;
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
      if (!this.IsInteractive)
        return false;
      XAttribute xattribute = this.source as XAttribute ?? this.parent as XAttribute;
      if (xattribute == null || xattribute.parent == null)
        return false;
      this.source = (object)xattribute.parent;
      this.parent = (object)null;
      return true;
    }

    public override bool MoveToFirstAttribute()
    {
      if (!this.IsInteractive)
        return false;
      XElement inAttributeScope = this.GetElementInAttributeScope();
      if (inAttributeScope == null || inAttributeScope.lastAttr == null)
        return false;
      if (this.omitDuplicateNamespaces)
      {
        object obj = (object)this.GetFirstNonDuplicateNamespaceAttribute(inAttributeScope.lastAttr.next);
        if (obj == null)
          return false;
        this.source = obj;
      }
      else
        this.source = (object)inAttributeScope.lastAttr.next;
      return true;
    }

    public override bool MoveToNextAttribute()
    {
      if (!this.IsInteractive)
        return false;
      XElement xelement = this.source as XElement;
      if (xelement != null)
      {
        if (this.IsEndElement || xelement.lastAttr == null)
          return false;
        if (this.omitDuplicateNamespaces)
        {
          object obj = (object)this.GetFirstNonDuplicateNamespaceAttribute(xelement.lastAttr.next);
          if (obj == null)
            return false;
          this.source = obj;
        }
        else
          this.source = (object)xelement.lastAttr.next;
        return true;
      }
      XAttribute xattribute = this.source as XAttribute ?? this.parent as XAttribute;
      if (xattribute == null || xattribute.parent == null || ((XElement)xattribute.parent).lastAttr == xattribute)
        return false;
      if (this.omitDuplicateNamespaces)
      {
        object obj = (object)this.GetFirstNonDuplicateNamespaceAttribute(xattribute.next);
        if (obj == null)
          return false;
        this.source = obj;
      }
      else
        this.source = (object)xattribute.next;
      this.parent = (object)null;
      return true;
    }

    public override bool Read()
    {
      switch (this.state)
      {
        case ReadState.Initial:
        this.state = ReadState.Interactive;
        XDocument d = this.source as XDocument;
        if (d != null)
          return this.ReadIntoDocument(d);
        return true;
        case ReadState.Interactive:
        return this.Read(false);
        default:
        return false;
      }
    }

    public override bool ReadAttributeValue()
    {
      if (!this.IsInteractive)
        return false;
      XAttribute a = this.source as XAttribute;
      if (a != null)
        return this.ReadIntoAttribute(a);
      return false;
    }

    public override bool ReadToDescendant(string localName, string namespaceName)
    {
      if (!this.IsInteractive)
        return false;
      this.MoveToElement();
      XElement xelement1 = this.source as XElement;
      if (xelement1 != null && !xelement1.IsEmpty && !this.IsEndElement)
      {
        foreach (XElement xelement2 in xelement1.Descendants())
        {
          if (xelement2.Name.LocalName == localName && xelement2.Name.NamespaceName == namespaceName)
          {
            this.source = (object)xelement2;
            return true;
          }
        }
        this.IsEndElement = true;
      }
      return false;
    }

    public override bool ReadToFollowing(string localName, string namespaceName)
    {
      while (this.Read())
      {
        XElement xelement = this.source as XElement;
        if (xelement != null && !this.IsEndElement && (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName))
          return true;
      }
      return false;
    }

    public override bool ReadToNextSibling(string localName, string namespaceName)
    {
      if (!this.IsInteractive)
        return false;
      this.MoveToElement();
      if (this.source != this.root)
      {
        XNode xnode = this.source as XNode;
        if (xnode != null)
        {
          foreach (XElement xelement in xnode.ElementsAfterSelf())
          {
            if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
            {
              this.source = (object)xelement;
              this.IsEndElement = false;
              return true;
            }
          }
          if (xnode.parent is XElement)
          {
            this.source = (object)xnode.parent;
            this.IsEndElement = true;
            return false;
          }
        }
        else if (this.parent is XElement)
        {
          this.source = this.parent;
          this.parent = (object)null;
          this.IsEndElement = true;
          return false;
        }
      }
      return this.ReadToEnd();
    }

    public override void ResolveEntity()
    {
    }

    public override void Skip()
    {
      if (!this.IsInteractive)
        return;
      this.Read(true);
    }

    bool IXmlLineInfo.HasLineInfo()
    {
      if (this.IsEndElement)
      {
        XElement xelement = this.source as XElement;
        if (xelement != null)
          return xelement.Annotation<LineInfoEndElementAnnotation>() != null;
      }
      else
      {
        IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
        if (xmlLineInfo != null)
          return xmlLineInfo.HasLineInfo();
      }
      return false;
    }

    private static XmlNameTable CreateNameTable()
    {
      XmlNameTable xmlNameTable = (XmlNameTable)new System.Xml.NameTable();
      xmlNameTable.Add(string.Empty);
      xmlNameTable.Add("http://www.w3.org/2000/xmlns/");
      xmlNameTable.Add("http://www.w3.org/XML/1998/namespace");
      return xmlNameTable;
    }

    private XElement GetElementInAttributeScope()
    {
      XElement xelement = this.source as XElement;
      if (xelement != null)
      {
        if (this.IsEndElement)
          return (XElement)null;
        return xelement;
      }
      XAttribute xattribute1 = this.source as XAttribute;
      if (xattribute1 != null)
        return (XElement)xattribute1.parent;
      XAttribute xattribute2 = this.parent as XAttribute;
      if (xattribute2 != null)
        return (XElement)xattribute2.parent;
      return (XElement)null;
    }

    private XElement GetElementInScope()
    {
      XElement xelement1 = this.source as XElement;
      if (xelement1 != null)
        return xelement1;
      XNode xnode = this.source as XNode;
      if (xnode != null)
        return xnode.parent as XElement;
      XAttribute xattribute1 = this.source as XAttribute;
      if (xattribute1 != null)
        return (XElement)xattribute1.parent;
      XElement xelement2 = this.parent as XElement;
      if (xelement2 != null)
        return xelement2;
      XAttribute xattribute2 = this.parent as XAttribute;
      if (xattribute2 != null)
        return (XElement)xattribute2.parent;
      return (XElement)null;
    }

    private static void GetNameInAttributeScope(string qualifiedName, XElement e, out string localName, out string namespaceName)
    {
      if (qualifiedName != null && qualifiedName.Length != 0)
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
          XNamespace namespaceOfPrefix = e.GetNamespaceOfPrefix(qualifiedName.Substring(0, length));
          if (namespaceOfPrefix != (XNamespace)null)
          {
            localName = qualifiedName.Substring(length + 1, qualifiedName.Length - length - 1);
            namespaceName = namespaceOfPrefix.NamespaceName;
            return;
          }
        }
      }
      localName = (string)null;
      namespaceName = (string)null;
    }

    private bool Read(bool skipContent)
    {
      XElement e = this.source as XElement;
      if (e != null)
      {
        if (((e.IsEmpty ? 1 : (this.IsEndElement ? 1 : 0)) | (skipContent ? 1 : 0)) != 0)
          return this.ReadOverNode((XNode)e);
        return this.ReadIntoElement(e);
      }
      XNode n = this.source as XNode;
      if (n != null)
        return this.ReadOverNode(n);
      XAttribute a = this.source as XAttribute;
      if (a != null)
        return this.ReadOverAttribute(a, skipContent);
      return this.ReadOverText(skipContent);
    }

    private bool ReadIntoDocument(XDocument d)
    {
      XNode xnode = d.content as XNode;
      if (xnode != null)
      {
        this.source = (object)xnode.next;
        return true;
      }
      string str = d.content as string;
      if (str == null || str.Length <= 0)
        return this.ReadToEnd();
      this.source = (object)str;
      this.parent = (object)d;
      return true;
    }

    private bool ReadIntoElement(XElement e)
    {
      XNode xnode = e.content as XNode;
      if (xnode != null)
      {
        this.source = (object)xnode.next;
        return true;
      }
      string str = e.content as string;
      if (str == null)
        return this.ReadToEnd();
      if (str.Length > 0)
      {
        this.source = (object)str;
        this.parent = (object)e;
      }
      else
      {
        this.source = (object)e;
        this.IsEndElement = true;
      }
      return true;
    }

    private bool ReadIntoAttribute(XAttribute a)
    {
      this.source = (object)a.value;
      this.parent = (object)a;
      return true;
    }

    private bool ReadOverAttribute(XAttribute a, bool skipContent)
    {
      XElement e = (XElement)a.parent;
      if (e == null)
        return this.ReadToEnd();
      if (e.IsEmpty | skipContent)
        return this.ReadOverNode((XNode)e);
      return this.ReadIntoElement(e);
    }

    private bool ReadOverNode(XNode n)
    {
      if (n == this.root)
        return this.ReadToEnd();
      XNode xnode = n.next;
      if (xnode == null || xnode == n || n == n.parent.content)
      {
        if (n.parent == null || n.parent.parent == null && n.parent is XDocument)
          return this.ReadToEnd();
        this.source = (object)n.parent;
        this.IsEndElement = true;
      }
      else
      {
        this.source = (object)xnode;
        this.IsEndElement = false;
      }
      return true;
    }

    private bool ReadOverText(bool skipContent)
    {
      if (this.parent is XElement)
      {
        this.source = this.parent;
        this.parent = (object)null;
        this.IsEndElement = true;
        return true;
      }
      if (!(this.parent is XAttribute))
        return this.ReadToEnd();
      XAttribute a = (XAttribute)this.parent;
      this.parent = (object)null;
      return this.ReadOverAttribute(a, skipContent);
    }

    private bool ReadToEnd()
    {
      this.state = ReadState.EndOfFile;
      return false;
    }

    private bool IsDuplicateNamespaceAttribute(XAttribute candidateAttribute)
    {
      if (!candidateAttribute.IsNamespaceDeclaration)
        return false;
      return this.IsDuplicateNamespaceAttributeInner(candidateAttribute);
    }

    private bool IsDuplicateNamespaceAttributeInner(XAttribute candidateAttribute)
    {
      if (candidateAttribute.Name.LocalName == "xml")
        return true;
      XElement xelement1 = candidateAttribute.parent as XElement;
      if (xelement1 == this.root || xelement1 == null)
        return false;
      for (XElement xelement2 = xelement1.parent as XElement; xelement2 != null; xelement2 = xelement2.parent as XElement)
      {
        XAttribute xattribute = xelement2.lastAttr;
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
        if (xelement2 == this.root)
          return false;
      }
      return false;
    }

    private XAttribute GetFirstNonDuplicateNamespaceAttribute(XAttribute candidate)
    {
      if (!this.IsDuplicateNamespaceAttribute(candidate))
        return candidate;
      XElement xelement = candidate.parent as XElement;
      if (xelement != null && candidate != xelement.lastAttr)
      {
        do
        {
          candidate = candidate.next;
          if (!this.IsDuplicateNamespaceAttribute(candidate))
            return candidate;
        }
        while (candidate != xelement.lastAttr);
      }
      return (XAttribute)null;
    }
  }
}
