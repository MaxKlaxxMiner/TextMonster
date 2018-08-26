using System.Xml;

namespace TextMonster.Xml
{
  internal struct ElementWriter
  {
    private XmlWriter writer;
    private NamespaceResolver resolver;

    public ElementWriter(XmlWriter writer)
    {
      this.writer = writer;
      this.resolver = new NamespaceResolver();
    }

    public void WriteElement(XElement e)
    {
      this.PushAncestors(e);
      XElement xelement = e;
      XNode xnode = (XNode)e;
      while (true)
      {
        e = xnode as XElement;
        if (e != null)
        {
          this.WriteStartElement(e);
          if (e.content == null)
          {
            this.WriteEndElement();
          }
          else
          {
            string text = e.content as string;
            if (text != null)
            {
              this.writer.WriteString(text);
              this.WriteFullEndElement();
            }
            else
            {
              xnode = ((XNode)e.content).next;
              continue;
            }
          }
        }
        else
          xnode.WriteTo(this.writer);
        while (xnode != xelement && xnode == xnode.parent.content)
        {
          xnode = (XNode)xnode.parent;
          this.WriteFullEndElement();
        }
        if (xnode != xelement)
          xnode = xnode.next;
        else
          break;
      }
    }

    private string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
    {
      string namespaceName = ns.NamespaceName;
      if (namespaceName.Length == 0)
        return string.Empty;
      string prefixOfNamespace = this.resolver.GetPrefixOfNamespace(ns, allowDefaultNamespace);
      if (prefixOfNamespace != null)
        return prefixOfNamespace;
      if (namespaceName == "http://www.w3.org/XML/1998/namespace")
        return "xml";
      if (namespaceName == "http://www.w3.org/2000/xmlns/")
        return "xmlns";
      return (string)null;
    }

    private void PushAncestors(XElement e)
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
          this.resolver.AddFirst(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
      }
      while (xattribute != e.lastAttr);
      goto label_0;
    }

    private void PushElement(XElement e)
    {
      this.resolver.PushScope();
      XAttribute xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
        if (xattribute.IsNamespaceDeclaration)
          this.resolver.Add(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
      }
      while (xattribute != e.lastAttr);
    }

    private void WriteEndElement()
    {
      this.writer.WriteEndElement();
      this.resolver.PopScope();
    }

    private void WriteFullEndElement()
    {
      this.writer.WriteFullEndElement();
      this.resolver.PopScope();
    }

    private void WriteStartElement(XElement e)
    {
      this.PushElement(e);
      XNamespace namespace1 = e.Name.Namespace;
      this.writer.WriteStartElement(this.GetPrefixOfNamespace(namespace1, true), e.Name.LocalName, namespace1.NamespaceName);
      XAttribute xattribute = e.lastAttr;
      if (xattribute == null)
        return;
      do
      {
        xattribute = xattribute.next;
        XNamespace namespace2 = xattribute.Name.Namespace;
        string localName = xattribute.Name.LocalName;
        string namespaceName = namespace2.NamespaceName;
        this.writer.WriteAttributeString(this.GetPrefixOfNamespace(namespace2, false), localName, namespaceName.Length != 0 || !(localName == "xmlns") ? namespaceName : "http://www.w3.org/2000/xmlns/", xattribute.Value);
      }
      while (xattribute != e.lastAttr);
    }
  }
}
