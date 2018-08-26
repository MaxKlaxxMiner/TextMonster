using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace TextMonster.Xml
{
  internal struct StreamingElementWriter
  {
    readonly XmlWriter writer;
    X_StreamingElement element;
    readonly List<X_Attribute> attributes;
    NamespaceResolver resolver;

    public StreamingElementWriter(XmlWriter w)
    {
      writer = w;
      element = null;
      attributes = new List<X_Attribute>();
      resolver = new NamespaceResolver();
    }

    void FlushElement()
    {
      if (element == null)
        return;
      PushElement();
      var namespace1 = element.Name.Namespace;
      writer.WriteStartElement(GetPrefixOfNamespace(namespace1, true), element.Name.LocalName, namespace1.NamespaceName);
      foreach (var xattribute in attributes)
      {
        var namespace2 = xattribute.Name.Namespace;
        string localName = xattribute.Name.LocalName;
        string namespaceName = namespace2.NamespaceName;
        writer.WriteAttributeString(GetPrefixOfNamespace(namespace2, false), localName, namespaceName.Length != 0 || localName != "xmlns" ? namespaceName : "http://www.w3.org/2000/xmlns/", xattribute.Value);
      }
      element = null;
      attributes.Clear();
    }

    string GetPrefixOfNamespace(X_Namespace ns, bool allowDefaultNamespace)
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

    void PushElement()
    {
      resolver.PushScope();
      foreach (var xattribute in attributes)
      {
        if (xattribute.IsNamespaceDeclaration)
          resolver.Add(xattribute.Name.NamespaceName.Length == 0 ? string.Empty : xattribute.Name.LocalName, X_Namespace.Get(xattribute.Value));
      }
    }

    void Write(object content)
    {
      if (content == null)
        return;
      var n = content as X_Node;
      if (n != null)
      {
        WriteNode(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          WriteString(s);
        }
        else
        {
          var a = content as X_Attribute;
          if (a != null)
          {
            WriteAttribute(a);
          }
          else
          {
            var e = content as X_StreamingElement;
            if (e != null)
            {
              WriteStreamingElement(e);
            }
            else
            {
              var objArray = content as object[];
              if (objArray != null)
              {
                foreach (var content1 in objArray)
                  Write(content1);
              }
              else
              {
                var enumerable = content as IEnumerable;
                if (enumerable != null)
                {
                  foreach (var content1 in enumerable)
                    Write(content1);
                }
                else
                  WriteString(X_Container.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    void WriteAttribute(X_Attribute a)
    {
      if (element == null)
        throw new InvalidOperationException("InvalidOperation_WriteAttribute");
      attributes.Add(a);
    }

    void WriteNode(X_Node n)
    {
      FlushElement();
      n.WriteTo(writer);
    }

    internal void WriteStreamingElement(X_StreamingElement e)
    {
      FlushElement();
      element = e;
      Write(e.content);
      bool flag = element == null;
      FlushElement();
      if (flag)
        writer.WriteFullEndElement();
      else
        writer.WriteEndElement();
      resolver.PopScope();
    }

    void WriteString(string s)
    {
      FlushElement();
      writer.WriteString(s);
    }
  }
}
