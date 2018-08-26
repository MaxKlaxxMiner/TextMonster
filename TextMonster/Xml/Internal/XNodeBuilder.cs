using System;
using System.Collections.Generic;
using System.Xml;

namespace TextMonster.Xml
{
  internal class XNodeBuilder : XmlWriter
  {
    List<object> content;
    X_Container parent;
    X_Name attrName;
    string attrValue;
    readonly X_Container root;

    public override XmlWriterSettings Settings
    {
      get
      {
        return new XmlWriterSettings
        {
          ConformanceLevel = ConformanceLevel.Auto
        };
      }
    }

    public override WriteState WriteState
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public XNodeBuilder(X_Container container)
    {
      root = container;
    }

    public override void Close()
    {
      root.Add(content);
    }

    public override void Flush()
    {
    }

    public override string LookupPrefix(string namespaceName)
    {
      throw new NotSupportedException();
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException("NotSupported_WriteBase64");
    }

    public override void WriteCData(string text)
    {
      AddNode(new X_CData(text));
    }

    public override void WriteCharEntity(char ch)
    {
      AddString(new string(ch, 1));
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      AddString(new string(buffer, index, count));
    }

    public override void WriteComment(string text)
    {
      AddNode(new X_Comment(text));
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      AddNode(new X_DocumentType(name, pubid, sysid, subset));
    }

    public override void WriteEndAttribute()
    {
      var xattribute = new X_Attribute(attrName, attrValue);
      attrName = null;
      attrValue = null;
      if (parent != null)
        parent.Add(xattribute);
      else
        Add(xattribute);
    }

    public override void WriteEndDocument()
    {
    }

    public override void WriteEndElement()
    {
      parent = parent.parent;
    }

    public override void WriteEntityRef(string name)
    {
      if (name != "amp")
      {
        if (name != "apos")
        {
          if (name != "gt")
          {
            if (name != "lt")
            {
              if (name != "quot")
                throw new NotSupportedException("NotSupported_WriteEntityRef");
              AddString("\"");
            }
            else
              AddString("<");
          }
          else
            AddString(">");
        }
        else
          AddString("'");
      }
      else
        AddString("&");
    }

    public override void WriteFullEndElement()
    {
      var xelement = (X_Element)parent;
      if (xelement.IsEmpty)
        xelement.Add(string.Empty);
      parent = xelement.parent;
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      if (name == "xml")
        return;
      AddNode(new X_ProcessingInstruction(name, text));
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      AddString(new string(buffer, index, count));
    }

    public override void WriteRaw(string data)
    {
      AddString(data);
    }

    public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
    {
      if (prefix == null)
        throw new ArgumentNullException("prefix");
      attrName = X_Namespace.Get(prefix.Length == 0 ? string.Empty : namespaceName).GetName(localName);
      attrValue = string.Empty;
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteStartDocument(bool standalone)
    {
    }

    public override void WriteStartElement(string prefix, string localName, string namespaceName)
    {
      AddNode(new X_Element(X_Namespace.Get(namespaceName).GetName(localName)));
    }

    public override void WriteString(string text)
    {
      AddString(text);
    }

    public override void WriteSurrogateCharEntity(char lowCh, char highCh)
    {
      AddString(new string(new[]
      {
        highCh,
        lowCh
      }));
    }

    public override void WriteValue(DateTimeOffset value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    public override void WriteWhitespace(string ws)
    {
      AddString(ws);
    }

    void Add(object o)
    {
      if (content == null)
        content = new List<object>();
      content.Add(o);
    }

    void AddNode(X_Node n)
    {
      if (parent != null)
        parent.Add(n);
      else
        Add(n);
      var xcontainer = n as X_Container;
      if (xcontainer == null)
        return;
      parent = xcontainer;
    }

    void AddString(string s)
    {
      if (s == null)
        return;
      if (attrValue != null)
        attrValue = attrValue + s;
      else if (parent != null)
        parent.Add(s);
      else
        Add(s);
    }
  }
}
