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

    public StreamingElementWriter(XmlWriter w)
    {
      writer = w;
      element = null;
      attributes = new List<X_Attribute>();
    }

    void FlushElement()
    {
      if (element == null)
        return;
      writer.WriteStartElement(string.Empty, element.Name, string.Empty);
      foreach (var xattribute in attributes)
      {
        writer.WriteAttributeString(string.Empty, xattribute.Name, string.Empty, xattribute.Value);
      }
      element = null;
      attributes.Clear();
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
    }

    void WriteString(string s)
    {
      FlushElement();
      writer.WriteString(s);
    }
  }
}
