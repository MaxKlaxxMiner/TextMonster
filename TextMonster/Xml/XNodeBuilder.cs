using System;
using System.Collections.Generic;
using System.Xml;

namespace TextMonster.Xml
{
  internal class XNodeBuilder : XmlWriter
  {
    private List<object> content;
    private XContainer parent;
    private XName attrName;
    private string attrValue;
    private XContainer root;

    public override XmlWriterSettings Settings
    {
      get
      {
        return new XmlWriterSettings()
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

    public XNodeBuilder(XContainer container)
    {
      this.root = container;
    }

    public override void Close()
    {
      this.root.Add((object)this.content);
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
      this.AddNode((XNode)new XCData(text));
    }

    public override void WriteCharEntity(char ch)
    {
      this.AddString(new string(ch, 1));
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      this.AddString(new string(buffer, index, count));
    }

    public override void WriteComment(string text)
    {
      this.AddNode((XNode)new XComment(text));
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      this.AddNode((XNode)new XDocumentType(name, pubid, sysid, subset));
    }

    public override void WriteEndAttribute()
    {
      XAttribute xattribute = new XAttribute(this.attrName, (object)this.attrValue);
      this.attrName = (XName)null;
      this.attrValue = (string)null;
      if (this.parent != null)
        this.parent.Add((object)xattribute);
      else
        this.Add((object)xattribute);
    }

    public override void WriteEndDocument()
    {
    }

    public override void WriteEndElement()
    {
      this.parent = this.parent.parent;
    }

    public override void WriteEntityRef(string name)
    {
      if (!(name == "amp"))
      {
        if (!(name == "apos"))
        {
          if (!(name == "gt"))
          {
            if (!(name == "lt"))
            {
              if (!(name == "quot"))
                throw new NotSupportedException("NotSupported_WriteEntityRef");
              this.AddString("\"");
            }
            else
              this.AddString("<");
          }
          else
            this.AddString(">");
        }
        else
          this.AddString("'");
      }
      else
        this.AddString("&");
    }

    public override void WriteFullEndElement()
    {
      XElement xelement = (XElement)this.parent;
      if (xelement.IsEmpty)
        xelement.Add((object)string.Empty);
      this.parent = xelement.parent;
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      if (name == "xml")
        return;
      this.AddNode((XNode)new XProcessingInstruction(name, text));
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      this.AddString(new string(buffer, index, count));
    }

    public override void WriteRaw(string data)
    {
      this.AddString(data);
    }

    public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
    {
      if (prefix == null)
        throw new ArgumentNullException("prefix");
      this.attrName = XNamespace.Get(prefix.Length == 0 ? string.Empty : namespaceName).GetName(localName);
      this.attrValue = string.Empty;
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteStartDocument(bool standalone)
    {
    }

    public override void WriteStartElement(string prefix, string localName, string namespaceName)
    {
      this.AddNode((XNode)new XElement(XNamespace.Get(namespaceName).GetName(localName)));
    }

    public override void WriteString(string text)
    {
      this.AddString(text);
    }

    public override void WriteSurrogateCharEntity(char lowCh, char highCh)
    {
      this.AddString(new string(new char[2]
      {
        highCh,
        lowCh
      }));
    }

    public override void WriteValue(DateTimeOffset value)
    {
      this.WriteString(XmlConvert.ToString(value));
    }

    public override void WriteWhitespace(string ws)
    {
      this.AddString(ws);
    }

    private void Add(object o)
    {
      if (this.content == null)
        this.content = new List<object>();
      this.content.Add(o);
    }

    private void AddNode(XNode n)
    {
      if (this.parent != null)
        this.parent.Add((object)n);
      else
        this.Add((object)n);
      XContainer xcontainer = n as XContainer;
      if (xcontainer == null)
        return;
      this.parent = xcontainer;
    }

    private void AddString(string s)
    {
      if (s == null)
        return;
      if (this.attrValue != null)
        this.attrValue = this.attrValue + s;
      else if (this.parent != null)
        this.parent.Add((object)s);
      else
        this.Add((object)s);
    }
  }
}
