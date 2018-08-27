using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Knoten dar, der weitere Knoten enthalten kann.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public abstract class X_Container : X_Node
  {
    internal object content;

    /// <summary>
    /// Ruft den ersten untergeordneten Knoten dieses Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNode"/>, der den ersten untergeordneten Knoten des <see cref="T:System.Xml.Linq.XContainer"/> enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public X_Node FirstNode
    {
      get
      {
        var lastNode = LastNode;
        if (lastNode == null)
          return null;
        return lastNode.next;
      }
    }

    /// <summary>
    /// Ruft den letzten untergeordneten Knoten dieses Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNode"/>, der den letzten untergeordneten Knoten des <see cref="T:System.Xml.Linq.XContainer"/> enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public X_Node LastNode
    {
      get
      {
        if (content == null)
          return null;
        var xnode = content as X_Node;
        if (xnode != null)
          return xnode;
        string str = content as string;
        if (str != null)
        {
          if (str.Length == 0)
            return null;
          var xtext = new X_Text(str) { parent = this };
          xtext.next = xtext;
          Interlocked.CompareExchange<object>(ref content, xtext, str);
        }
        return (X_Node)content;
      }
    }

    internal X_Container()
    {
    }

    internal X_Container(X_Container other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      if (other.content is string)
      {
        content = other.content;
      }
      else
      {
        var xnode = (X_Node)other.content;
        if (xnode == null)
          return;
        do
        {
          xnode = xnode.next;
          AppendNodeSkipNotify(xnode.CloneNode());
        }
        while (xnode != other.content);
      }
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als untergeordnete Elemente dieses <see cref="T:System.Xml.Linq.XContainer"/> hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die hinzugefügt werden sollen.</param>
    public void Add(object content)
    {
      if (SkipNotify())
      {
        AddContentSkipNotify(content);
      }
      else
      {
        if (content == null)
          return;
        var n = content as X_Node;
        if (n != null)
        {
          AddNode(n);
        }
        else
        {
          string s = content as string;
          if (s != null)
          {
            AddString(s);
          }
          else
          {
            var a = content as X_Attribute;
            if (a != null)
            {
              AddAttribute(a);
            }
            else
            {
              var other = content as X_StreamingElement;
              if (other != null)
              {
                AddNode(new X_Element(other));
              }
              else
              {
                var objArray = content as object[];
                if (objArray != null)
                {
                  foreach (var content1 in objArray)
                    Add(content1);
                }
                else
                {
                  var enumerable = content as IEnumerable;
                  if (enumerable != null)
                  {
                    foreach (var content1 in enumerable)
                      Add(content1);
                  }
                  else
                    AddString(GetStringValue(content));
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als untergeordnete Elemente dieses <see cref="T:System.Xml.Linq.XContainer"/> hinzu.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void Add(params object[] content)
    {
      Add((object)content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als erste untergeordnete Elemente dieses Dokuments oder Elements hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die hinzugefügt werden sollen.</param>
    public void AddFirst(object content)
    {
      new Inserter(this, null).Add(content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als erste untergeordnete Elemente dieses Dokuments oder Elements hinzu.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void AddFirst(params object[] content)
    {
      AddFirst((object)content);
    }

    /// <summary>
    /// Erstellt einen <see cref="T:System.Xml.XmlWriter"/>, der zum Hinzufügen von Knoten zu dem <see cref="T:System.Xml.Linq.XContainer"/> verwendet werden kann.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.XmlWriter"/>, in den Inhalt geschrieben werden kann.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XmlWriter CreateWriter()
    {
      return XmlWriter.Create(new XNodeBuilder(this), new XmlWriterSettings
      {
        ConformanceLevel = this is X_Document ? ConformanceLevel.Document : ConformanceLevel.Fragment
      });
    }

    /// <summary>
    /// Gibt eine Auflistung der Nachfolgerknoten für dieses Dokument oder Element in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/>, das die Nachfolgerknoten des <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    public IEnumerable<X_Node> DescendantNodes()
    {
      return GetDescendantNodes(false);
    }

    /// <summary>
    /// Gibt eine Auflistung der Nachfolgerelemente für dieses Dokument oder Element in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> mit den Nachfolgerelementen des <see cref="T:System.Xml.Linq.XContainer"/>.
    /// </returns>
    public IEnumerable<X_Element> Descendants()
    {
      return GetDescendants(null, false);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der Nachfolgerelemente für dieses Dokument oder Element in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die Nachfolgerelemente des <see cref="T:System.Xml.Linq.XContainer"/> enthält, die mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> übereinstimmen.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<X_Element> Descendants(string name)
    {
      if (name == null)
        return X_Element.EmptySequence;
      return GetDescendants(name, false);
    }

    /// <summary>
    /// Ruft das erste (in Dokumentreihenfolge) untergeordnete Element mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> übereinstimmt, oder null.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public X_Element Element(string name)
    {
      var xnode = content as X_Node;
      if (xnode != null)
      {
        do
        {
          xnode = xnode.next;
          var xelement = xnode as X_Element;
          if (xelement != null && xelement.name == name)
            return xelement;
        }
        while (xnode != content);
      }
      return null;
    }

    /// <summary>
    /// Gibt eine Auflistung der untergeordneten Elemente dieses Dokuments oder Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die untergeordneten Elemente dieses <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    public IEnumerable<X_Element> Elements()
    {
      return GetElements(null);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der untergeordneten Elemente dieses Dokuments oder Elements in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die untergeordneten Elemente des <see cref="T:System.Xml.Linq.XContainer"/>, die einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> aufweisen, in Dokumentreihenfolge enthält.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<X_Element> Elements(string name)
    {
      if (name == null)
        return X_Element.EmptySequence;
      return GetElements(name);
    }

    /// <summary>
    /// Gibt eine Auflistung der untergeordneten Knoten dieses Dokuments oder Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/>, das die Inhalte dieses <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    public IEnumerable<X_Node> Nodes()
    {
      var n = LastNode;
      if (n != null)
      {
        do
        {
          n = n.next;
          yield return n;
        }
        while (n.parent == this && n != content);
      }
    }

    /// <summary>
    /// Entfernt die untergeordneten Knoten aus diesem Dokument oder Element.
    /// </summary>
    public void RemoveNodes()
    {
      if (SkipNotify())
      {
        RemoveNodesSkipNotify();
      }
      else
      {
        while (content != null)
        {
          string str = content as string;
          if (str != null)
          {
            if (str.Length > 0)
              ConvertTextToNode();
            else if (this is X_Element)
            {
              NotifyChanging(this, X_ObjectChangeEventArgs.Value);
              if (!ReferenceEquals(str, content))
                throw new InvalidOperationException("InvalidOperation_ExternalCode");
              content = null;
              NotifyChanged(this, X_ObjectChangeEventArgs.Value);
            }
            else
              content = null;
          }
          var xnode1 = content as X_Node;
          if (xnode1 != null)
          {
            var xnode2 = xnode1.next;
            NotifyChanging(xnode2, X_ObjectChangeEventArgs.Remove);
            if (xnode1 != content || xnode2 != xnode1.next)
              throw new InvalidOperationException("InvalidOperation_ExternalCode");
            if (xnode2 != xnode1)
              xnode1.next = xnode2.next;
            else
              content = null;
            xnode2.parent = null;
            xnode2.next = null;
            NotifyChanged(xnode2, X_ObjectChangeEventArgs.Remove);
          }
        }
      }
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten dieses Dokuments oder Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die die untergeordneten Knoten ersetzen.</param>
    public void ReplaceNodes(object content)
    {
      content = GetContentSnapshot(content);
      RemoveNodes();
      Add(content);
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten dieses Dokuments oder Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    public void ReplaceNodes(params object[] content)
    {
      ReplaceNodes((object)content);
    }

    internal virtual void AddAttribute(X_Attribute a)
    {
    }

    internal virtual void AddAttributeSkipNotify(X_Attribute a)
    {
    }

    internal void AddContentSkipNotify(object content)
    {
      if (content == null)
        return;
      var n = content as X_Node;
      if (n != null)
      {
        AddNodeSkipNotify(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          AddStringSkipNotify(s);
        }
        else
        {
          var a = content as X_Attribute;
          if (a != null)
          {
            AddAttributeSkipNotify(a);
          }
          else
          {
            var other = content as X_StreamingElement;
            if (other != null)
            {
              AddNodeSkipNotify(new X_Element(other));
            }
            else
            {
              var objArray = content as object[];
              if (objArray != null)
              {
                foreach (var content1 in objArray)
                  AddContentSkipNotify(content1);
              }
              else
              {
                var enumerable = content as IEnumerable;
                if (enumerable != null)
                {
                  foreach (var content1 in enumerable)
                    AddContentSkipNotify(content1);
                }
                else
                  AddStringSkipNotify(GetStringValue(content));
              }
            }
          }
        }
      }
    }

    internal void AddNode(X_Node n)
    {
      ValidateNode(n, this);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        var xnode = (X_Node)this;
        while (xnode.parent != null)
          xnode = xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      ConvertTextToNode();
      AppendNode(n);
    }

    internal void AddNodeSkipNotify(X_Node n)
    {
      ValidateNode(n, this);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        var xnode = (X_Node)this;
        while (xnode.parent != null)
          xnode = xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      ConvertTextToNode();
      AppendNodeSkipNotify(n);
    }

    internal void AddString(string s)
    {
      ValidateString(s);
      if (content == null)
      {
        if (s.Length > 0)
          AppendNode(new X_Text(s));
        else if (this is X_Element)
        {
          NotifyChanging(this, X_ObjectChangeEventArgs.Value);
          if (content != null)
            throw new InvalidOperationException("InvalidOperation_ExternalCode");
          content = s;
          NotifyChanged(this, X_ObjectChangeEventArgs.Value);
        }
        else
          content = s;
      }
      else
      {
        if (s.Length <= 0)
          return;
        ConvertTextToNode();
        var xtext = content as X_Text;
        if (xtext != null && !(xtext is X_CData))
          xtext.Value += s;
        else
          AppendNode(new X_Text(s));
      }
    }

    internal void AddStringSkipNotify(string s)
    {
      ValidateString(s);
      if (content == null)
      {
        content = s;
      }
      else
      {
        if (s.Length <= 0)
          return;
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

    internal void AppendNode(X_Node n)
    {
      bool flag = NotifyChanging(n, X_ObjectChangeEventArgs.Add);
      if (n.parent != null)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      AppendNodeSkipNotify(n);
      if (!flag)
        return;
      NotifyChanged(n, X_ObjectChangeEventArgs.Add);
    }

    internal void AppendNodeSkipNotify(X_Node n)
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

    internal override void AppendText(StringBuilder sb)
    {
      string str = content as string;
      if (str != null)
      {
        sb.Append(str);
      }
      else
      {
        var xnode = (X_Node)content;
        if (xnode == null)
          return;
        do
        {
          xnode = xnode.next;
          xnode.AppendText(sb);
        }
        while (xnode != content);
      }
    }

    string GetTextOnly()
    {
      if (content == null)
        return null;
      string str = content as string;
      if (str == null)
      {
        var xnode = (X_Node)content;
        do
        {
          xnode = xnode.next;
          if (xnode.NodeType != XmlNodeType.Text)
            return null;
          str += ((X_Text)xnode).Value;
        }
        while (xnode != content);
      }
      return str;
    }

    string CollectText(ref X_Node n)
    {
      string str = "";
      while (n != null && n.NodeType == XmlNodeType.Text)
      {
        str += ((X_Text)n).Value;
        n = n != content ? n.next : null;
      }
      return str;
    }

    internal bool ContentsEqual(X_Container e)
    {
      if (content == e.content)
        return true;
      string textOnly = GetTextOnly();
      if (textOnly != null)
        return textOnly == e.GetTextOnly();
      var xnode1 = content as X_Node;
      var xnode2 = e.content as X_Node;
      if (xnode1 != null && xnode2 != null)
      {
        var n1 = xnode1.next;
        for (var n2 = xnode2.next; CollectText(ref n1) == e.CollectText(ref n2); n2 = n2 != e.content ? n2.next : (X_Node)null)
        {
          if (n1 == null && n2 == null)
            return true;
          if (n1 != null && n2 != null && n1.DeepEquals(n2))
            n1 = n1 != content ? n1.next : null;
          else
            break;
        }
      }
      return false;
    }

    internal int ContentsHashCode()
    {
      string textOnly = GetTextOnly();
      if (textOnly != null)
        return textOnly.GetHashCode();
      int num = 0;
      var n = content as X_Node;
      if (n != null)
      {
        do
        {
          n = n.next;
          string str = CollectText(ref n);
          if (str.Length > 0)
            num ^= str.GetHashCode();
          if (n != null)
            num ^= n.GetDeepHashCode();
          else
            break;
        }
        while (n != content);
      }
      return num;
    }

    internal void ConvertTextToNode()
    {
      string str = content as string;
      if (str == null || str.Length <= 0)
        return;
      var xtext = new X_Text(str) { parent = this };
      xtext.next = xtext;
      content = xtext;
    }

    internal static string GetDateTimeString(DateTime value)
    {
      return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
    }

    internal IEnumerable<X_Node> GetDescendantNodes(bool self)
    {
      if (self)
        yield return this;
      var n = (X_Node)this;
      while (true)
      {
        var xcontainer = n as X_Container;
        X_Node firstNode;
        if (xcontainer != null && (firstNode = xcontainer.FirstNode) != null)
        {
          n = firstNode;
        }
        else
        {
          while (n != null && n != this && n == n.parent.content)
            n = n.parent;
          if (n != null && n != this)
            n = n.next;
          else
            break;
        }
        yield return n;
      }
    }

    internal IEnumerable<X_Element> GetDescendants(string name, bool self)
    {
      if (self)
      {
        var xelement = (X_Element)this;
        if (name == null || xelement.name == name)
          yield return xelement;
      }
      var n = (X_Node)this;
      var xcontainer = this;
      while (true)
      {
        if (xcontainer != null && xcontainer.content is X_Node)
        {
          n = ((X_Node)xcontainer.content).next;
        }
        else
        {
          while (n != this && n == n.parent.content)
            n = n.parent;
          if (n != this)
            n = n.next;
          else
            break;
        }
        var e = n as X_Element;
        if (e != null && (name == null || e.name == name))
          yield return e;
        xcontainer = e;
        e = null;
      }
    }

    IEnumerable<X_Element> GetElements(string name)
    {
      var n = content as X_Node;
      if (n != null)
      {
        do
        {
          n = n.next;
          var xelement = n as X_Element;
          if (xelement != null && (name == null || xelement.name == name))
            yield return xelement;
        }
        while (n.parent == this && n != content);
      }
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
        if (value is X_Object)
          throw new ArgumentException("Argument_XObjectValue");
        str = value.ToString();
      }
      if (str == null)
        throw new ArgumentException("Argument_ConvertToString");
      return str;
    }

    internal void ReadContentFrom(XmlReader r)
    {
      if (r.ReadState != ReadState.Interactive)
        throw new InvalidOperationException("InvalidOperation_ExpectedInteractive");
      var xcontainer = this;
      do
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
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
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          xcontainer.AddStringSkipNotify(r.Value);
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.CDATA:
          xcontainer.AddNodeSkipNotify(new X_CData(r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EntityReference:
          if (!r.CanResolveEntity)
            throw new InvalidOperationException("InvalidOperation_UnresolvedEntityReference");
          r.ResolveEntity();
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.ProcessingInstruction:
          xcontainer.AddNodeSkipNotify(new X_ProcessingInstruction(r.Name, r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.Comment:
          xcontainer.AddNodeSkipNotify(new X_Comment(r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.DocumentType:
          xcontainer.AddNodeSkipNotify(new X_DocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EndElement:
          if (xcontainer.content == null)
            xcontainer.content = string.Empty;
          if (xcontainer == this)
            return;
          xcontainer = xcontainer.parent;
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EndEntity:
          continue;
          default:
          throw new InvalidOperationException("InvalidOperation_UnexpectedNodeType");
        }
      }
      while (r.Read());
    }

    internal void ReadContentFrom(XmlReader r, LoadOptions o)
    {
      if ((o & (LoadOptions.SetBaseUri | LoadOptions.SetLineInfo)) == LoadOptions.None)
      {
        ReadContentFrom(r);
      }
      else
      {
        if (r.ReadState != ReadState.Interactive)
          throw new InvalidOperationException("InvalidOperation_ExpectedInteractive");
        var xcontainer = this;
        var n = (X_Node)null;
        string str = (o & LoadOptions.SetBaseUri) != LoadOptions.None ? r.BaseURI : null;
        var xmlLineInfo = (o & LoadOptions.SetLineInfo) != LoadOptions.None ? r as IXmlLineInfo : null;
        do
        {
          string baseUri = r.BaseURI;
          switch (r.NodeType)
          {
            case XmlNodeType.Element:
            var xelement1 = new X_Element(r.LocalName);
            if (str != null && str != baseUri)
              xelement1.SetBaseUri(baseUri);
            if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
              xelement1.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
            if (r.MoveToFirstAttribute())
            {
              do
              {
                var a = new X_Attribute(r.LocalName, r.Value);
                if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
                  a.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
                xelement1.AppendAttributeSkipNotify(a);
              }
              while (r.MoveToNextAttribute());
              r.MoveToElement();
            }
            xcontainer.AddNodeSkipNotify(xelement1);
            if (!r.IsEmptyElement)
            {
              xcontainer = xelement1;
              if (str != null)
              {
                str = baseUri;
              }
            }
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.Text:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
            if (str != null && str != baseUri || xmlLineInfo != null && xmlLineInfo.HasLineInfo())
            {
              n = new X_Text(r.Value);
              goto case XmlNodeType.EndEntity;
            }
            xcontainer.AddStringSkipNotify(r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.CDATA:
            n = new X_CData(r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.EntityReference:
            if (!r.CanResolveEntity)
              throw new InvalidOperationException("InvalidOperation_UnresolvedEntityReference");
            r.ResolveEntity();
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.ProcessingInstruction:
            n = new X_ProcessingInstruction(r.Name, r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.Comment:
            n = new X_Comment(r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.DocumentType:
            n = new X_DocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.EndElement:
            if (xcontainer.content == null)
              xcontainer.content = string.Empty;
            var xelement2 = xcontainer as X_Element;
            if (xelement2 != null && xmlLineInfo != null && xmlLineInfo.HasLineInfo())
              xelement2.SetEndElementLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
            if (xcontainer == this)
              return;
            if (str != null && xcontainer.HasBaseUri)
              str = xcontainer.parent.BaseUri;
            xcontainer = xcontainer.parent;
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.EndEntity:
            if (n != null)
            {
              if (str != null && str != baseUri)
                n.SetBaseUri(baseUri);
              if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
                n.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
              xcontainer.AddNodeSkipNotify(n);
              n = null;
            }
            continue;
            default:
            throw new InvalidOperationException("InvalidOperation_UnexpectedNodeType");
          }
        }
        while (r.Read());
      }
    }

    internal void RemoveNode(X_Node n)
    {
      bool flag = NotifyChanging(n, X_ObjectChangeEventArgs.Remove);
      if (n.parent != this)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      var xnode = (X_Node)content;
      while (xnode.next != n)
        xnode = xnode.next;
      if (xnode == n)
      {
        content = null;
      }
      else
      {
        if (content == n)
          content = xnode;
        xnode.next = n.next;
      }
      n.parent = null;
      n.next = null;
      if (!flag)
        return;
      NotifyChanged(n, X_ObjectChangeEventArgs.Remove);
    }

    void RemoveNodesSkipNotify()
    {
      var xnode1 = content as X_Node;
      if (xnode1 != null)
      {
        do
        {
          var xnode2 = xnode1.next;
          xnode1.parent = null;
          xnode1.next = null;
          xnode1 = xnode2;
        }
        while (xnode1 != content);
      }
      content = null;
    }

    internal virtual void ValidateNode(X_Node node, X_Node previous)
    {
    }

    internal virtual void ValidateString(string s)
    {
    }

    internal void WriteContentTo(XmlWriter writer)
    {
      if (content == null)
        return;
      if (content is string)
      {
        if (this is X_Document)
          writer.WriteWhitespace((string)content);
        else
          writer.WriteString((string)content);
      }
      else
      {
        var xnode = (X_Node)content;
        do
        {
          xnode = xnode.next;
          xnode.WriteTo(writer);
        }
        while (xnode != content);
      }
    }

    static void AddContentToList(List<object> list, object content)
    {
      var enumerable = content is string ? null : content as IEnumerable;
      if (enumerable == null)
      {
        list.Add(content);
      }
      else
      {
        foreach (var content1 in enumerable)
        {
          if (content1 != null)
            AddContentToList(list, content1);
        }
      }
    }

    internal static object GetContentSnapshot(object content)
    {
      if (content is string || !(content is IEnumerable))
        return content;
      var list = new List<object>();
      AddContentToList(list, content);
      return list;
    }
  }
}
