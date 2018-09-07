﻿using System;
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

    public abstract NameTable NameTable { get; }

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

    public abstract bool MoveToParent();

    public virtual IXmlSchemaInfo SchemaInfo
    {
      get { return this as IXmlSchemaInfo; }
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
