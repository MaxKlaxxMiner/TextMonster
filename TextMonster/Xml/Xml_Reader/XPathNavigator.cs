using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TextMonster.Xml.Xml_Reader
{
  [DebuggerDisplay("{debuggerDisplayProxy}")]
  public abstract class XPathNavigator : XPathItem, ICloneable, IXPathNavigable, IXmlNamespaceResolver
  {
    public override string ToString()
    {
      return Value;
    }

    public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
    {
      if (nsResolver == null)
      {
        nsResolver = this;
      }
      IXmlSchemaInfo schemaInfo = SchemaInfo;
      XmlSchemaType schemaType;
      XmlSchemaDatatype datatype;
      if (schemaInfo != null)
      {
        if (schemaInfo.Validity == XmlSchemaValidity.Valid)
        {
          schemaType = schemaInfo.MemberType;
          if (schemaType == null)
          {
            schemaType = schemaInfo.SchemaType;
          }
          if (schemaType != null)
          {
            return schemaType.ValueConverter.ChangeType(Value, returnType, nsResolver);
          }
        }
        else
        {
          schemaType = schemaInfo.SchemaType;
          if (schemaType != null)
          {
            datatype = schemaType.Datatype;
            if (datatype != null)
            {
              return schemaType.ValueConverter.ChangeType(datatype.ParseValue(Value, NameTable, nsResolver), returnType, nsResolver);
            }
          }
        }
      }
      return XmlUntypedConverter.Untyped.ChangeType(Value, returnType, nsResolver);
    }

    //-----------------------------------------------
    // ICloneable
    //-----------------------------------------------

    object ICloneable.Clone()
    {
      return Clone();
    }

    //-----------------------------------------------
    // IXPathNavigable
    //-----------------------------------------------

    public virtual XPathNavigator CreateNavigator()
    {
      return Clone();
    }

    //-----------------------------------------------
    // IXmlNamespaceResolver
    //-----------------------------------------------

    public abstract XmlNameTable NameTable { get; }

    public virtual string LookupNamespace(string prefix)
    {
      if (prefix == null)
        return null;

      if (NodeType != XPathNodeType.Element)
      {
        XPathNavigator navSave = Clone();

        // If current item is not an element, then try parent
        if (navSave.MoveToParent())
          return navSave.LookupNamespace(prefix);
      }
      else if (MoveToNamespace(prefix))
      {
        string namespaceURI = Value;
        MoveToParent();
        return namespaceURI;
      }

      // Check for "", "xml", and "xmlns" prefixes
      if (prefix.Length == 0)
        return string.Empty;
      else if (prefix == "xml")
        return XmlReservedNs.NsXml;
      else if (prefix == "xmlns")
        return XmlReservedNs.NsXmlNs;

      return null;
    }

    public virtual string LookupPrefix(string namespaceURI)
    {
      if (namespaceURI == null)
        return null;

      XPathNavigator navClone = Clone();

      if (NodeType != XPathNodeType.Element)
      {
        // If current item is not an element, then try parent
        if (navClone.MoveToParent())
          return navClone.LookupPrefix(namespaceURI);
      }
      else
      {
        if (navClone.MoveToFirstNamespace(XPathNamespaceScope.All))
        {
          // Loop until a matching namespace is found
          do
          {
            if (namespaceURI == navClone.Value)
              return navClone.LocalName;
          }
          while (navClone.MoveToNextNamespace(XPathNamespaceScope.All));
        }
      }

      // Check for default, "xml", and "xmlns" namespaces
      if (namespaceURI == LookupNamespace(string.Empty))
        return string.Empty;
      else if (namespaceURI == XmlReservedNs.NsXml)
        return "xml";
      else if (namespaceURI == XmlReservedNs.NsXmlNs)
        return "xmlns";

      return null;
    }

    // This pragma disables a warning that the return type is not CLS-compliant, but generics are part of CLS in Whidbey. 
#pragma warning disable 3002
    public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
    {
#pragma warning restore 3002
      XPathNodeType nt = NodeType;
      if ((nt != XPathNodeType.Element && scope != XmlNamespaceScope.Local) || nt == XPathNodeType.Attribute || nt == XPathNodeType.Namespace)
      {
        XPathNavigator navSave = Clone();

        // If current item is not an element, then try parent
        if (navSave.MoveToParent())
          return navSave.GetNamespacesInScope(scope);
      }

      Dictionary<string, string> dict = new Dictionary<string, string>();

      // "xml" prefix always in scope
      if (scope == XmlNamespaceScope.All)
        dict["xml"] = XmlReservedNs.NsXml;

      // Now add all in-scope namespaces
      if (MoveToFirstNamespace((XPathNamespaceScope)scope))
      {
        do
        {
          string prefix = LocalName;
          string ns = Value;

          // Exclude xmlns="" declarations unless scope = Local
          if (prefix.Length != 0 || ns.Length != 0 || scope == XmlNamespaceScope.Local)
            dict[prefix] = ns;
        }
        while (MoveToNextNamespace((XPathNamespaceScope)scope));

        MoveToParent();
      }

      return dict;
    }

    public abstract XPathNavigator Clone();

    public abstract XPathNodeType NodeType { get; }

    public abstract string LocalName { get; }

    public abstract string Name { get; }

    public abstract string NamespaceURI { get; }

    public virtual object UnderlyingObject
    {
      get { return null; }
    }

    public virtual bool MoveToNamespace(string name)
    {
      if (MoveToFirstNamespace(XPathNamespaceScope.All))
      {

        do
        {
          if (name == LocalName)
            return true;
        }
        while (MoveToNextNamespace(XPathNamespaceScope.All));

        MoveToParent();
      }

      return false;
    }

    public abstract bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope);

    public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);

    public abstract bool MoveToNext();

    public abstract bool MoveToFirstChild();

    public abstract bool MoveToParent();

    public abstract bool MoveTo(XPathNavigator other);

    public virtual bool MoveToChild(string localName, string namespaceURI)
    {
      if (MoveToFirstChild())
      {
        do
        {
          if (NodeType == XPathNodeType.Element && localName == LocalName && namespaceURI == NamespaceURI)
            return true;
        }
        while (MoveToNext());
        MoveToParent();
      }

      return false;
    }

    public virtual bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
    {
      XPathNavigator navSave = Clone();

      if (end != null)
      {
        switch (end.NodeType)
        {
          case XPathNodeType.Attribute:
          case XPathNodeType.Namespace:
          // Scan until we come to the next content-typed node 
          // after the attribute or namespace node
          end = end.Clone();
          end.MoveToNonDescendant();
          break;
        }
      }
      switch (NodeType)
      {
        case XPathNodeType.Attribute:
        case XPathNodeType.Namespace:
        if (!MoveToParent())
        {
          // Restore previous position and return false
          // MoveTo(navSave);
          return false;
        }
        break;
      }
      do
      {
        if (!MoveToFirstChild())
        {
          // Look for next sibling
          while (true)
          {
            if (MoveToNext())
              break;

            if (!MoveToParent())
            {
              // Restore previous position and return false
              MoveTo(navSave);
              return false;
            }
          }
        }

        // Have we reached the end of the scan?
        if (end != null && IsSamePosition(end))
        {
          // Restore previous position and return false
          MoveTo(navSave);
          return false;
        }
      }
      while (NodeType != XPathNodeType.Element
             || localName != LocalName
             || namespaceURI != NamespaceURI);

      return true;
    }

    public virtual bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
    {
      XPathNavigator navSave = Clone();
      int mask = GetContentKindMask(type);

      if (end != null)
      {
        switch (end.NodeType)
        {
          case XPathNodeType.Attribute:
          case XPathNodeType.Namespace:
          // Scan until we come to the next content-typed node 
          // after the attribute or namespace node
          end = end.Clone();
          end.MoveToNonDescendant();
          break;
        }
      }
      switch (NodeType)
      {
        case XPathNodeType.Attribute:
        case XPathNodeType.Namespace:
        if (!MoveToParent())
        {
          // Restore previous position and return false
          // MoveTo(navSave);
          return false;
        }
        break;
      }
      do
      {
        if (!MoveToFirstChild())
        {
          // Look for next sibling
          while (true)
          {
            if (MoveToNext())
              break;

            if (!MoveToParent())
            {
              // Restore previous position and return false
              MoveTo(navSave);
              return false;
            }
          }
        }

        // Have we reached the end of the scan?
        if (end != null && IsSamePosition(end))
        {
          // Restore previous position and return false
          MoveTo(navSave);
          return false;
        }
      }
      while (((1 << (int)NodeType) & mask) == 0);

      return true;
    }

    public virtual bool MoveToNext(string localName, string namespaceURI)
    {
      XPathNavigator navClone = Clone();

      while (MoveToNext())
      {
        if (NodeType == XPathNodeType.Element && localName == LocalName && namespaceURI == NamespaceURI)
          return true;
      }
      MoveTo(navClone);
      return false;
    }

    public abstract bool IsSamePosition(XPathNavigator other);

    public virtual IXmlSchemaInfo SchemaInfo
    {
      get { return this as IXmlSchemaInfo; }
    }

    internal bool MoveToNonDescendant()
    {
      // If current node is document, there is no next non-descendant
      if (NodeType == XPathNodeType.Root)
        return false;

      // If sibling exists, it is the next non-descendant
      if (MoveToNext())
        return true;

      // The current node is either an attribute, namespace, or last child node
      XPathNavigator navSave = Clone();

      if (!MoveToParent())
        return false;

      switch (navSave.NodeType)
      {
        case XPathNodeType.Attribute:
        case XPathNodeType.Namespace:
        // Next node in document order is first content-child of parent
        if (MoveToFirstChild())
          return true;
        break;
      }

      while (!MoveToNext())
      {
        if (!MoveToParent())
        {
          // Restore original position and return false
          MoveTo(navSave);
          return false;
        }
      }

      return true;
    }

    internal const int AllMask = 0x7FFFFFFF;
    internal const int NoAttrNmspMask = AllMask & ~(1 << (int)XPathNodeType.Attribute) & ~(1 << (int)XPathNodeType.Namespace);
    internal const int TextMask = (1 << (int)XPathNodeType.Text) | (1 << (int)XPathNodeType.SignificantWhitespace) | (1 << (int)XPathNodeType.Whitespace);
    internal static readonly int[] ContentKindMasks = {
            (1 << (int) XPathNodeType.Root),                        // Root
            (1 << (int) XPathNodeType.Element),                     // Element
            0,                                                      // Attribute (not content)
            0,                                                      // Namespace (not content)
            TextMask,                                               // Text
            (1 << (int) XPathNodeType.SignificantWhitespace),       // SignificantWhitespace
            (1 << (int) XPathNodeType.Whitespace),                  // Whitespace
            (1 << (int) XPathNodeType.ProcessingInstruction),       // ProcessingInstruction
            (1 << (int) XPathNodeType.Comment),                     // Comment
            NoAttrNmspMask,                                         // All
        };

    internal static int GetContentKindMask(XPathNodeType type)
    {
      return ContentKindMasks[(int)type];
    }

    internal static int GetKindMask(XPathNodeType type)
    {
      if (type == XPathNodeType.All)
        return AllMask;
      else if (type == XPathNodeType.Text)
        return TextMask;

      return (1 << (int)type);
    }

    internal static bool IsText(XPathNodeType type)
    {
      //return ((1 << (int) type) & TextMask) != 0;
      return (uint)(type - XPathNodeType.Text) <= (XPathNodeType.Whitespace - XPathNodeType.Text);
    }

    private object debuggerDisplayProxy { get { return new DebuggerDisplayProxy(this); } }

    [DebuggerDisplay("{ToString()}")]
    internal struct DebuggerDisplayProxy
    {
      XPathNavigator nav;
      public DebuggerDisplayProxy(XPathNavigator nav)
      {
        this.nav = nav;
      }
      public override string ToString()
      {
        string result = nav.NodeType.ToString();
        switch (nav.NodeType)
        {
          case XPathNodeType.Element:
          result += ", Name=\"" + nav.Name + '"';
          break;
          case XPathNodeType.Attribute:
          case XPathNodeType.Namespace:
          case XPathNodeType.ProcessingInstruction:
          result += ", Name=\"" + nav.Name + '"';
          result += ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(nav.Value) + '"';
          break;
          case XPathNodeType.Text:
          case XPathNodeType.Whitespace:
          case XPathNodeType.SignificantWhitespace:
          case XPathNodeType.Comment:
          result += ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(nav.Value) + '"';
          break;
        }
        return result;
      }
    }
  }
}
