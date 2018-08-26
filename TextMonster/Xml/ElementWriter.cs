﻿using System.Xml;

namespace TextMonster.Xml
{
  internal struct ElementWriter
  {
    readonly XmlWriter writer;
    NamespaceResolver resolver;

    public ElementWriter(XmlWriter writer)
    {
      this.writer = writer;
      resolver = new NamespaceResolver();
    }

    public void WriteElement(XElement e)
    {
      PushAncestors(e);
      var xelement = e;
      var xnode = (XNode)e;
      while (true)
      {
        e = xnode as XElement;
        if (e != null)
        {
          WriteStartElement(e);
          if (e.content == null)
          {
            WriteEndElement();
          }
          else
          {
            string text = e.content as string;
            if (text != null)
            {
              writer.WriteString(text);
              WriteFullEndElement();
            }
            else
            {
              xnode = ((XNode)e.content).next;
              continue;
            }
          }
        }
        else
          xnode.WriteTo(writer);
        while (xnode != xelement && xnode == xnode.parent.content)
        {
          xnode = xnode.parent;
          WriteFullEndElement();
        }
        if (xnode != xelement)
          xnode = xnode.next;
        else
          break;
      }
    }

    string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
    {
      string namespaceName = ns.NamespaceName;
      if (namespaceName.Length == 0)
        return string.Empty;
      string prefixOfNamespace = resolver.GetPrefixOfNamespace(ns, allowDefaultNamespace);
      if (prefixOfNamespace != null)
        return prefixOfNamespace;
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
        return "xml";
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return null;
    }

    void PushAncestors(XElement e)
    {
    label_0:
      XAttribute xattribute;
      do
      {
        e = e.parent as XElement;
        if (e != null)
          xattribute = e.lastAttr;
        else
          goto label_5;
      }
      while (xattribute == null);
      goto label_2;
    label_5:
      return;
    label_2:
      do
      {
        xattribute = xattribute.next;
        if (xattribute.IsNamespaceDeclaration)
          resolver.AddFirst(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
      }
      while (xattribute != e.lastAttr);
      goto label_0;
    }

    void PushElement(XElement e)
    {
      resolver.PushScope();
      var xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
        if (xattribute.IsNamespaceDeclaration)
          resolver.Add(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
      }
      while (xattribute != e.lastAttr);
    }

    void WriteEndElement()
    {
      writer.WriteEndElement();
      resolver.PopScope();
    }

    void WriteFullEndElement()
    {
      writer.WriteFullEndElement();
      resolver.PopScope();
    }

    void WriteStartElement(XElement e)
    {
      PushElement(e);
      var namespace1 = e.Name.Namespace;
      writer.WriteStartElement(GetPrefixOfNamespace(namespace1, true), e.Name.LocalName, namespace1.NamespaceName);
      var xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
        var namespace2 = xattribute.Name.Namespace;
        string localName = xattribute.Name.LocalName;
        string namespaceName = namespace2.NamespaceName;
        writer.WriteAttributeString(GetPrefixOfNamespace(namespace2, false), localName, namespaceName.Length != 0 || localName != "xmlns" ? namespaceName : "http://www.w3.org/2000/xmlns/", xattribute.Value);
      }
      while (xattribute != e.lastAttr);
    }
  }
}
