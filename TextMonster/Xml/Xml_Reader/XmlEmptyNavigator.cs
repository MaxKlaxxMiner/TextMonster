namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// The XmlEmptyNavigator exposes a document node with no children.
  /// Only one XmlEmptyNavigator exists per AppDomain (Singleton).  That's why the constructor is private.
  /// Use the Singleton property to get the EmptyNavigator.
  /// </summary>
  internal class XmlEmptyNavigator : XPathNavigator
  {
    private static volatile XmlEmptyNavigator singleton;

    private XmlEmptyNavigator()
    {
    }

    public static XmlEmptyNavigator Singleton
    {
      get
      {
        if (XmlEmptyNavigator.singleton == null)
          XmlEmptyNavigator.singleton = new XmlEmptyNavigator();
        return XmlEmptyNavigator.singleton;
      }
    }

    //-----------------------------------------------
    // XmlReader
    //-----------------------------------------------

    public override XPathNodeType NodeType
    {
      get { return XPathNodeType.All; }
    }

    public override string NamespaceURI
    {
      get { return string.Empty; }
    }

    public override string LocalName
    {
      get { return string.Empty; }
    }

    public override string Name
    {
      get { return string.Empty; }
    }

    public override string Prefix
    {
      get { return string.Empty; }
    }

    public override string BaseURI
    {
      get { return string.Empty; }
    }

    public override string Value
    {
      get { return string.Empty; }
    }

    public override bool IsEmptyElement
    {
      get { return false; }
    }

    public override string XmlLang
    {
      get { return string.Empty; }
    }


    //-----------------------------------------------
    // IXmlNamespaceResolver
    //-----------------------------------------------

    public override XmlNameTable NameTable
    {
      get { return new NameTable(); }
    }

    public override bool MoveToFirstChild()
    {
      return false;
    }

    public override void MoveToRoot()
    {
      //always on root
      return;
    }

    public override bool MoveToNext()
    {
      return false;
    }

    public override bool MoveToFirstAttribute()
    {
      return false;
    }

    public override bool MoveToNextAttribute()
    {
      return false;
    }

    public override bool MoveToId(string id)
    {
      return false;
    }

    public override bool MoveToAttribute(string localName, string namespaceName)
    {
      return false;
    }

    public override string GetNamespace(string name)
    {
      return null;
    }

    public override bool MoveToNamespace(string prefix)
    {
      return false;
    }


    public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
    {
      return false;
    }

    public override bool MoveToNextNamespace(XPathNamespaceScope scope)
    {
      return false;
    }

    public override bool MoveToParent()
    {
      return false;
    }

    public override bool MoveTo(XPathNavigator other)
    {
      // Only one instance of XmlEmptyNavigator exists on the system
      return (object)this == (object)other;
    }

    public override XmlNodeOrder ComparePosition(XPathNavigator other)
    {
      // Only one instance of XmlEmptyNavigator exists on the system
      return ((object)this == (object)other) ? XmlNodeOrder.Same : XmlNodeOrder.Unknown;
    }

    public override bool IsSamePosition(XPathNavigator other)
    {
      // Only one instance of XmlEmptyNavigator exists on the system
      return (object)this == (object)other;
    }


    //-----------------------------------------------
    // XPathNavigator2
    //-----------------------------------------------
    public override XPathNavigator Clone()
    {
      // Singleton, so clone just returns this
      return this;
    }
  }
}
