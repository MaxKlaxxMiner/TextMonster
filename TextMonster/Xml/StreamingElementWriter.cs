using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace TextMonster.Xml
{
  internal struct StreamingElementWriter
  {
    private XmlWriter writer;
    private XStreamingElement element;
    private List<XAttribute> attributes;
    private NamespaceResolver resolver;

    public StreamingElementWriter(XmlWriter w)
    {
      this.writer = w;
      this.element = (XStreamingElement)null;
      this.attributes = new List<XAttribute>();
      this.resolver = new NamespaceResolver();
    }

    private void FlushElement()
    {
      if (this.element == null)
        return;
      this.PushElement();
      XNamespace namespace1 = this.element.Name.Namespace;
      this.writer.WriteStartElement(this.GetPrefixOfNamespace(namespace1, true), this.element.Name.LocalName, namespace1.NamespaceName);
      foreach (XAttribute xattribute in this.attributes)
      {
        XNamespace namespace2 = xattribute.Name.Namespace;
        string localName = xattribute.Name.LocalName;
        string namespaceName = namespace2.NamespaceName;
        this.writer.WriteAttributeString(this.GetPrefixOfNamespace(namespace2, false), localName, namespaceName.Length != 0 || !(localName == "xmlns") ? namespaceName : "http://www.w3.org/2000/xmlns/", xattribute.Value);
      }
      this.element = (XStreamingElement)null;
      this.attributes.Clear();
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

    private void PushElement()
    {
      this.resolver.PushScope();
      foreach (XAttribute xattribute in this.attributes)
      {
        if (xattribute.IsNamespaceDeclaration)
          this.resolver.Add(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, XNamespace.Get(xattribute.Value));
      }
    }

    private void Write(object content)
    {
      if (content == null)
        return;
      XNode n = content as XNode;
      if (n != null)
      {
        this.WriteNode(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          this.WriteString(s);
        }
        else
        {
          XAttribute a = content as XAttribute;
          if (a != null)
          {
            this.WriteAttribute(a);
          }
          else
          {
            XStreamingElement e = content as XStreamingElement;
            if (e != null)
            {
              this.WriteStreamingElement(e);
            }
            else
            {
              object[] objArray = content as object[];
              if (objArray != null)
              {
                foreach (object content1 in objArray)
                  this.Write(content1);
              }
              else
              {
                IEnumerable enumerable = content as IEnumerable;
                if (enumerable != null)
                {
                  foreach (object content1 in enumerable)
                    this.Write(content1);
                }
                else
                  this.WriteString(XContainer.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    private void WriteAttribute(XAttribute a)
    {
      if (this.element == null)
        throw new InvalidOperationException("InvalidOperation_WriteAttribute");
      this.attributes.Add(a);
    }

    private void WriteNode(XNode n)
    {
      this.FlushElement();
      n.WriteTo(this.writer);
    }

    internal void WriteStreamingElement(XStreamingElement e)
    {
      this.FlushElement();
      this.element = e;
      this.Write(e.content);
      bool flag = this.element == null;
      this.FlushElement();
      if (flag)
        this.writer.WriteFullEndElement();
      else
        this.writer.WriteEndElement();
      this.resolver.PopScope();
    }

    private void WriteString(string s)
    {
      this.FlushElement();
      this.writer.WriteString(s);
    }
  }
}
