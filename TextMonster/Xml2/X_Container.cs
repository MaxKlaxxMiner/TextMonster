using System;
using System.Xml;
// ReSharper disable InconsistentNaming
// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace ngMax.Xml
{
  /// <summary>
  /// Klasse zum halten eines Xml-Kontainers
  /// </summary>
  public abstract class X_Container : X_Node
  {
    internal object content;

    internal X_Container()
    {
    }

    internal X_Container(X_Container other)
    {
      if (other.content is string)
      {
        content = other.content;
      }
      else
      {
        var xnode = (X_Node)other.content;
        if (xnode == null) return;
        do
        {
          xnode = xnode.next;
          AppendNodeSkipNotify(xnode.CloneNode());
        }
        while (xnode != other.content);
      }
    }

    static string GetDateTimeString(DateTime value)
    {
      return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
    }

    internal static string GetStringValue(object value)
    {
      string str;
      if (value is string)
        str = (string)value;
      else if (value is double)
        str = XmlConvert.ToString((double)value);
      else if (value is float)
        str = XmlConvert.ToString((float)value);
      else if (value is decimal)
        str = XmlConvert.ToString((decimal)value);
      else if (value is bool)
        str = XmlConvert.ToString((bool)value);
      else if (value is DateTime)
        str = GetDateTimeString((DateTime)value);
      else if (value is DateTimeOffset)
        str = XmlConvert.ToString((DateTimeOffset)value);
      else if (value is TimeSpan)
      {
        str = XmlConvert.ToString((TimeSpan)value);
      }
      else
      {
        if (value is X_Object) throw new ArgumentException("Argument_XObjectValue");
        str = value.ToString();
      }
      if (str == null) throw new ArgumentException("Argument_ConvertToString");
      return str;
    }

    void ConvertTextToNode()
    {
      string str = content as string;
      if (str == null || str.Length <= 0) return;
      var xtext = new X_Text(str) { parent = this };
      xtext.next = xtext;
      content = xtext;
    }

    void AppendNodeSkipNotify(X_Node n)
    {
      n.parent = this;
      if (content == null || content is string)
      {
        n.next = n;
      }
      else
      {
        var xnode = (X_Node)content;
        n.next = xnode.next;
        xnode.next = n;
      }
      content = n;
    }

    public void AddNodeSkipNotify(X_Node n)
    {
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        X_Node xnode = this;
        while (xnode.parent != null) xnode = xnode.parent;
        if (n == xnode) n = n.CloneNode();
      }
      ConvertTextToNode();
      AppendNodeSkipNotify(n);
    }

    void AddStringSkipNotify(string s)
    {
      // TODO this.ValidateString(s);
      if (content == null)
      {
        content = s;
      }
      else
      {
        if (s.Length <= 0) return;
        if (content is string)
        {
          content = (string)content + s;
        }
        else
        {
          var xtext = content as X_Text;
          if (xtext != null && !(xtext is X_CData))
            xtext.text += s;
          else
            AppendNodeSkipNotify(new X_Text(s));
        }
      }
    }

    internal void ReadContentFrom(XmlReader r)
    {
      var xcontainer = this;
      do
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
          {
            var xelement = new X_Element(r.LocalName);
            if (r.MoveToFirstAttribute())
            {
              do
              {
                xelement.AppendAttributeSkipNotify(new X_Attribute(r.LocalName, r.Value));
              }
              while (r.MoveToNextAttribute());
              r.MoveToElement();
            }
            xcontainer.AddNodeSkipNotify(xelement);
            if (!r.IsEmptyElement)
            {
              xcontainer = xelement;
            }
            goto case XmlNodeType.EndEntity;
          }
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace: xcontainer.AddStringSkipNotify(r.Value); goto case XmlNodeType.EndEntity;
          case XmlNodeType.CDATA: xcontainer.AddNodeSkipNotify(new X_CData(r.Value)); goto case XmlNodeType.EndEntity;
          case XmlNodeType.EntityReference: if (!r.CanResolveEntity) throw new InvalidOperationException("InvalidOperation_UnresolvedEntityReference"); r.ResolveEntity(); goto case XmlNodeType.EndEntity;
          case XmlNodeType.ProcessingInstruction: throw new NotImplementedException();
          case XmlNodeType.Comment: xcontainer.AddNodeSkipNotify(new X_Comment(r.Value)); goto case XmlNodeType.EndEntity;
          case XmlNodeType.DocumentType: goto case XmlNodeType.EndEntity;
          case XmlNodeType.EndElement:
          {
            if (xcontainer.content == null) xcontainer.content = string.Empty;
            if (xcontainer == this) return;
            xcontainer = xcontainer.parent;
            goto case XmlNodeType.EndEntity;
          }
          case XmlNodeType.EndEntity: continue;
          case XmlNodeType.XmlDeclaration: continue;
          default: throw new InvalidOperationException("InvalidOperation_UnexpectedNodeType");
        }
      }
      while (r.Read());
    }
  }
}
