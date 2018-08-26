
namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Knoten dar, der weitere Knoten enthalten kann.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public abstract class XContainer : XNode
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
    [__DynamicallyInvokable]
    public XNode FirstNode
    {
      [__DynamicallyInvokable]
      get
      {
        XNode lastNode = this.LastNode;
        if (lastNode == null)
          return (XNode)null;
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
    [__DynamicallyInvokable]
    public XNode LastNode
    {
      [__DynamicallyInvokable]
      get
      {
        if (this.content == null)
          return (XNode)null;
        XNode xnode = this.content as XNode;
        if (xnode != null)
          return xnode;
        string str = this.content as string;
        if (str != null)
        {
          if (str.Length == 0)
            return (XNode)null;
          XText xtext = new XText(str);
          xtext.parent = this;
          xtext.next = (XNode)xtext;
          Interlocked.CompareExchange<object>(ref this.content, (object)xtext, (object)str);
        }
        return (XNode)this.content;
      }
    }

    internal XContainer()
    {
    }

    internal XContainer(XContainer other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      if (other.content is string)
      {
        this.content = other.content;
      }
      else
      {
        XNode xnode = (XNode)other.content;
        if (xnode == null)
          return;
        do
        {
          xnode = xnode.next;
          this.AppendNodeSkipNotify(xnode.CloneNode());
        }
        while (xnode != other.content);
      }
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als untergeordnete Elemente dieses <see cref="T:System.Xml.Linq.XContainer"/> hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die hinzugefügt werden sollen.</param>
    [__DynamicallyInvokable]
    public void Add(object content)
    {
      if (this.SkipNotify())
      {
        this.AddContentSkipNotify(content);
      }
      else
      {
        if (content == null)
          return;
        XNode n = content as XNode;
        if (n != null)
        {
          this.AddNode(n);
        }
        else
        {
          string s = content as string;
          if (s != null)
          {
            this.AddString(s);
          }
          else
          {
            XAttribute a = content as XAttribute;
            if (a != null)
            {
              this.AddAttribute(a);
            }
            else
            {
              XStreamingElement other = content as XStreamingElement;
              if (other != null)
              {
                this.AddNode((XNode)new XElement(other));
              }
              else
              {
                object[] objArray = content as object[];
                if (objArray != null)
                {
                  foreach (object content1 in objArray)
                    this.Add(content1);
                }
                else
                {
                  IEnumerable enumerable = content as IEnumerable;
                  if (enumerable != null)
                  {
                    foreach (object content1 in enumerable)
                      this.Add(content1);
                  }
                  else
                    this.AddString(XContainer.GetStringValue(content));
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
    [__DynamicallyInvokable]
    public void Add(params object[] content)
    {
      this.Add((object)content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als erste untergeordnete Elemente dieses Dokuments oder Elements hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die hinzugefügt werden sollen.</param>
    [__DynamicallyInvokable]
    public void AddFirst(object content)
    {
      new Inserter(this, (XNode)null).Add(content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt als erste untergeordnete Elemente dieses Dokuments oder Elements hinzu.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    [__DynamicallyInvokable]
    public void AddFirst(params object[] content)
    {
      this.AddFirst((object)content);
    }

    /// <summary>
    /// Erstellt einen <see cref="T:System.Xml.XmlWriter"/>, der zum Hinzufügen von Knoten zu dem <see cref="T:System.Xml.Linq.XContainer"/> verwendet werden kann.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.XmlWriter"/>, in den Inhalt geschrieben werden kann.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    [__DynamicallyInvokable]
    public XmlWriter CreateWriter()
    {
      return XmlWriter.Create((XmlWriter)new XNodeBuilder(this), new XmlWriterSettings()
      {
        ConformanceLevel = this is XDocument ? ConformanceLevel.Document : ConformanceLevel.Fragment
      });
    }

    /// <summary>
    /// Gibt eine Auflistung der Nachfolgerknoten für dieses Dokument oder Element in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/>, das die Nachfolgerknoten des <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    [__DynamicallyInvokable]
    public IEnumerable<XNode> DescendantNodes()
    {
      return this.GetDescendantNodes(false);
    }

    /// <summary>
    /// Gibt eine Auflistung der Nachfolgerelemente für dieses Dokument oder Element in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> mit den Nachfolgerelementen des <see cref="T:System.Xml.Linq.XContainer"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public IEnumerable<XElement> Descendants()
    {
      return this.GetDescendants((XName)null, false);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der Nachfolgerelemente für dieses Dokument oder Element in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die Nachfolgerelemente des <see cref="T:System.Xml.Linq.XContainer"/> enthält, die mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> übereinstimmen.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    [__DynamicallyInvokable]
    public IEnumerable<XElement> Descendants(XName name)
    {
      if (!(name != (XName)null))
        return XElement.EmptySequence;
      return this.GetDescendants(name, false);
    }

    /// <summary>
    /// Ruft das erste (in Dokumentreihenfolge) untergeordnete Element mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XElement"/>, das mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/> übereinstimmt, oder null.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    [__DynamicallyInvokable]
    public XElement Element(XName name)
    {
      XNode xnode = this.content as XNode;
      if (xnode != null)
      {
        do
        {
          xnode = xnode.next;
          XElement xelement = xnode as XElement;
          if (xelement != null && xelement.name == name)
            return xelement;
        }
        while (xnode != this.content);
      }
      return (XElement)null;
    }

    /// <summary>
    /// Gibt eine Auflistung der untergeordneten Elemente dieses Dokuments oder Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die untergeordneten Elemente dieses <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    [__DynamicallyInvokable]
    public IEnumerable<XElement> Elements()
    {
      return this.GetElements((XName)null);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der untergeordneten Elemente dieses Dokuments oder Elements in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/>, das die untergeordneten Elemente des <see cref="T:System.Xml.Linq.XContainer"/>, die einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> aufweisen, in Dokumentreihenfolge enthält.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    [__DynamicallyInvokable]
    public IEnumerable<XElement> Elements(XName name)
    {
      if (!(name != (XName)null))
        return XElement.EmptySequence;
      return this.GetElements(name);
    }

    /// <summary>
    /// Gibt eine Auflistung der untergeordneten Knoten dieses Dokuments oder Elements in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/>, das die Inhalte dieses <see cref="T:System.Xml.Linq.XContainer"/> in Dokumentreihenfolge enthält.
    /// </returns>
    [__DynamicallyInvokable]
    public IEnumerable<XNode> Nodes()
    {
      XNode n = this.LastNode;
      if (n != null)
      {
        do
        {
          n = n.next;
          yield return n;
        }
        while (n.parent == this && n != this.content);
      }
    }

    /// <summary>
    /// Entfernt die untergeordneten Knoten aus diesem Dokument oder Element.
    /// </summary>
    [__DynamicallyInvokable]
    public void RemoveNodes()
    {
      if (this.SkipNotify())
      {
        this.RemoveNodesSkipNotify();
      }
      else
      {
        while (this.content != null)
        {
          string str = this.content as string;
          if (str != null)
          {
            if (str.Length > 0)
              this.ConvertTextToNode();
            else if (this is XElement)
            {
              this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
              if (str != this.content)
                throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
              this.content = (object)null;
              this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
            }
            else
              this.content = (object)null;
          }
          XNode xnode1 = this.content as XNode;
          if (xnode1 != null)
          {
            XNode xnode2 = xnode1.next;
            this.NotifyChanging((object)xnode2, XObjectChangeEventArgs.Remove);
            if (xnode1 != this.content || xnode2 != xnode1.next)
              throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
            if (xnode2 != xnode1)
              xnode1.next = xnode2.next;
            else
              this.content = (object)null;
            xnode2.parent = (XContainer)null;
            xnode2.next = (XNode)null;
            this.NotifyChanged((object)xnode2, XObjectChangeEventArgs.Remove);
          }
        }
      }
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten dieses Dokuments oder Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die die untergeordneten Knoten ersetzen.</param>
    [__DynamicallyInvokable]
    public void ReplaceNodes(object content)
    {
      content = XContainer.GetContentSnapshot(content);
      this.RemoveNodes();
      this.Add(content);
    }

    /// <summary>
    /// Ersetzt die untergeordneten Knoten dieses Dokuments oder Elements durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param>
    [__DynamicallyInvokable]
    public void ReplaceNodes(params object[] content)
    {
      this.ReplaceNodes((object)content);
    }

    internal virtual void AddAttribute(XAttribute a)
    {
    }

    internal virtual void AddAttributeSkipNotify(XAttribute a)
    {
    }

    internal void AddContentSkipNotify(object content)
    {
      if (content == null)
        return;
      XNode n = content as XNode;
      if (n != null)
      {
        this.AddNodeSkipNotify(n);
      }
      else
      {
        string s = content as string;
        if (s != null)
        {
          this.AddStringSkipNotify(s);
        }
        else
        {
          XAttribute a = content as XAttribute;
          if (a != null)
          {
            this.AddAttributeSkipNotify(a);
          }
          else
          {
            XStreamingElement other = content as XStreamingElement;
            if (other != null)
            {
              this.AddNodeSkipNotify((XNode)new XElement(other));
            }
            else
            {
              object[] objArray = content as object[];
              if (objArray != null)
              {
                foreach (object content1 in objArray)
                  this.AddContentSkipNotify(content1);
              }
              else
              {
                IEnumerable enumerable = content as IEnumerable;
                if (enumerable != null)
                {
                  foreach (object content1 in enumerable)
                    this.AddContentSkipNotify(content1);
                }
                else
                  this.AddStringSkipNotify(XContainer.GetStringValue(content));
              }
            }
          }
        }
      }
    }

    internal void AddNode(XNode n)
    {
      this.ValidateNode(n, (XNode)this);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        XNode xnode = (XNode)this;
        while (xnode.parent != null)
          xnode = (XNode)xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      this.ConvertTextToNode();
      this.AppendNode(n);
    }

    internal void AddNodeSkipNotify(XNode n)
    {
      this.ValidateNode(n, (XNode)this);
      if (n.parent != null)
      {
        n = n.CloneNode();
      }
      else
      {
        XNode xnode = (XNode)this;
        while (xnode.parent != null)
          xnode = (XNode)xnode.parent;
        if (n == xnode)
          n = n.CloneNode();
      }
      this.ConvertTextToNode();
      this.AppendNodeSkipNotify(n);
    }

    internal void AddString(string s)
    {
      this.ValidateString(s);
      if (this.content == null)
      {
        if (s.Length > 0)
          this.AppendNode((XNode)new XText(s));
        else if (this is XElement)
        {
          this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
          if (this.content != null)
            throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
          this.content = (object)s;
          this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
        }
        else
          this.content = (object)s;
      }
      else
      {
        if (s.Length <= 0)
          return;
        this.ConvertTextToNode();
        XText xtext = this.content as XText;
        if (xtext != null && !(xtext is XCData))
          xtext.Value += s;
        else
          this.AppendNode((XNode)new XText(s));
      }
    }

    internal void AddStringSkipNotify(string s)
    {
      this.ValidateString(s);
      if (this.content == null)
      {
        this.content = (object)s;
      }
      else
      {
        if (s.Length <= 0)
          return;
        if (this.content is string)
        {
          this.content = (object)((string)this.content + s);
        }
        else
        {
          XText xtext = this.content as XText;
          if (xtext != null && !(xtext is XCData))
            xtext.text += s;
          else
            this.AppendNodeSkipNotify((XNode)new XText(s));
        }
      }
    }

    internal void AppendNode(XNode n)
    {
      bool flag = this.NotifyChanging((object)n, XObjectChangeEventArgs.Add);
      if (n.parent != null)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
      this.AppendNodeSkipNotify(n);
      if (!flag)
        return;
      this.NotifyChanged((object)n, XObjectChangeEventArgs.Add);
    }

    internal void AppendNodeSkipNotify(XNode n)
    {
      n.parent = this;
      if (this.content == null || this.content is string)
      {
        n.next = n;
      }
      else
      {
        XNode xnode = (XNode)this.content;
        n.next = xnode.next;
        xnode.next = n;
      }
      this.content = (object)n;
    }

    internal override void AppendText(StringBuilder sb)
    {
      string str = this.content as string;
      if (str != null)
      {
        sb.Append(str);
      }
      else
      {
        XNode xnode = (XNode)this.content;
        if (xnode == null)
          return;
        do
        {
          xnode = xnode.next;
          xnode.AppendText(sb);
        }
        while (xnode != this.content);
      }
    }

    private string GetTextOnly()
    {
      if (this.content == null)
        return (string)null;
      string str = this.content as string;
      if (str == null)
      {
        XNode xnode = (XNode)this.content;
        do
        {
          xnode = xnode.next;
          if (xnode.NodeType != XmlNodeType.Text)
            return (string)null;
          str += ((XText)xnode).Value;
        }
        while (xnode != this.content);
      }
      return str;
    }

    private string CollectText(ref XNode n)
    {
      string str = "";
      while (n != null && n.NodeType == XmlNodeType.Text)
      {
        str += ((XText)n).Value;
        n = n != this.content ? n.next : (XNode)null;
      }
      return str;
    }

    internal bool ContentsEqual(XContainer e)
    {
      if (this.content == e.content)
        return true;
      string textOnly = this.GetTextOnly();
      if (textOnly != null)
        return textOnly == e.GetTextOnly();
      XNode xnode1 = this.content as XNode;
      XNode xnode2 = e.content as XNode;
      if (xnode1 != null && xnode2 != null)
      {
        XNode n1 = xnode1.next;
        for (XNode n2 = xnode2.next; !(this.CollectText(ref n1) != e.CollectText(ref n2)); n2 = n2 != e.content ? n2.next : (XNode)null)
        {
          if (n1 == null && n2 == null)
            return true;
          if (n1 != null && n2 != null && n1.DeepEquals(n2))
            n1 = n1 != this.content ? n1.next : (XNode)null;
          else
            break;
        }
      }
      return false;
    }

    internal int ContentsHashCode()
    {
      string textOnly = this.GetTextOnly();
      if (textOnly != null)
        return textOnly.GetHashCode();
      int num = 0;
      XNode n = this.content as XNode;
      if (n != null)
      {
        do
        {
          n = n.next;
          string str = this.CollectText(ref n);
          if (str.Length > 0)
            num ^= str.GetHashCode();
          if (n != null)
            num ^= n.GetDeepHashCode();
          else
            break;
        }
        while (n != this.content);
      }
      return num;
    }

    internal void ConvertTextToNode()
    {
      string str = this.content as string;
      if (str == null || str.Length <= 0)
        return;
      XText xtext = new XText(str);
      xtext.parent = this;
      xtext.next = (XNode)xtext;
      this.content = (object)xtext;
    }

    internal static string GetDateTimeString(DateTime value)
    {
      return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
    }

    internal IEnumerable<XNode> GetDescendantNodes(bool self)
    {
      if (self)
        yield return (XNode)this;
      XNode n = (XNode)this;
      while (true)
      {
        XContainer xcontainer = n as XContainer;
        XNode firstNode;
        if (xcontainer != null && (firstNode = xcontainer.FirstNode) != null)
        {
          n = firstNode;
        }
        else
        {
          while (n != null && n != this && n == n.parent.content)
            n = (XNode)n.parent;
          if (n != null && n != this)
            n = n.next;
          else
            break;
        }
        yield return n;
      }
    }

    internal IEnumerable<XElement> GetDescendants(XName name, bool self)
    {
      if (self)
      {
        XElement xelement = (XElement)this;
        if (name == (XName)null || xelement.name == name)
          yield return xelement;
      }
      XNode n = (XNode)this;
      XContainer xcontainer = this;
      while (true)
      {
        if (xcontainer != null && xcontainer.content is XNode)
        {
          n = ((XNode)xcontainer.content).next;
        }
        else
        {
          while (n != this && n == n.parent.content)
            n = (XNode)n.parent;
          if (n != this)
            n = n.next;
          else
            break;
        }
        XElement e = n as XElement;
        if (e != null && (name == (XName)null || e.name == name))
          yield return e;
        xcontainer = (XContainer)e;
        e = (XElement)null;
      }
    }

    private IEnumerable<XElement> GetElements(XName name)
    {
      XNode n = this.content as XNode;
      if (n != null)
      {
        do
        {
          n = n.next;
          XElement xelement = n as XElement;
          if (xelement != null && (name == (XName)null || xelement.name == name))
            yield return xelement;
        }
        while (n.parent == this && n != this.content);
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
      else if (value is Decimal)
        str = XmlConvert.ToString((Decimal)value);
      else if (value is bool)
        str = XmlConvert.ToString((bool)value);
      else if (value is DateTime)
        str = XContainer.GetDateTimeString((DateTime)value);
      else if (value is DateTimeOffset)
        str = XmlConvert.ToString((DateTimeOffset)value);
      else if (value is TimeSpan)
      {
        str = XmlConvert.ToString((TimeSpan)value);
      }
      else
      {
        if (value is XObject)
          throw new ArgumentException(Res.GetString("Argument_XObjectValue"));
        str = value.ToString();
      }
      if (str == null)
        throw new ArgumentException(Res.GetString("Argument_ConvertToString"));
      return str;
    }

    internal void ReadContentFrom(XmlReader r)
    {
      if (r.ReadState != ReadState.Interactive)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
      XContainer xcontainer = this;
      NamespaceCache namespaceCache1 = new NamespaceCache();
      NamespaceCache namespaceCache2 = new NamespaceCache();
      do
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
          XElement xelement = new XElement(namespaceCache1.Get(r.NamespaceURI).GetName(r.LocalName));
          if (r.MoveToFirstAttribute())
          {
            do
            {
              xelement.AppendAttributeSkipNotify(new XAttribute(namespaceCache2.Get(r.Prefix.Length == 0 ? string.Empty : r.NamespaceURI).GetName(r.LocalName), (object)r.Value));
            }
            while (r.MoveToNextAttribute());
            r.MoveToElement();
          }
          xcontainer.AddNodeSkipNotify((XNode)xelement);
          if (!r.IsEmptyElement)
          {
            xcontainer = (XContainer)xelement;
            goto case XmlNodeType.EndEntity;
          }
          else
            goto case XmlNodeType.EndEntity;
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          xcontainer.AddStringSkipNotify(r.Value);
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.CDATA:
          xcontainer.AddNodeSkipNotify((XNode)new XCData(r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EntityReference:
          if (!r.CanResolveEntity)
            throw new InvalidOperationException(Res.GetString("InvalidOperation_UnresolvedEntityReference"));
          r.ResolveEntity();
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.ProcessingInstruction:
          xcontainer.AddNodeSkipNotify((XNode)new XProcessingInstruction(r.Name, r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.Comment:
          xcontainer.AddNodeSkipNotify((XNode)new XComment(r.Value));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.DocumentType:
          xcontainer.AddNodeSkipNotify((XNode)new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value, r.DtdInfo));
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EndElement:
          if (xcontainer.content == null)
            xcontainer.content = (object)string.Empty;
          if (xcontainer == this)
            return;
          xcontainer = xcontainer.parent;
          goto case XmlNodeType.EndEntity;
          case XmlNodeType.EndEntity:
          continue;
          default:
          throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedNodeType", new object[1]
            {
              (object) r.NodeType
            }));
        }
      }
      while (r.Read());
    }

    internal void ReadContentFrom(XmlReader r, LoadOptions o)
    {
      if ((o & (LoadOptions.SetBaseUri | LoadOptions.SetLineInfo)) == LoadOptions.None)
      {
        this.ReadContentFrom(r);
      }
      else
      {
        if (r.ReadState != ReadState.Interactive)
          throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
        XContainer xcontainer = this;
        XNode n = (XNode)null;
        NamespaceCache namespaceCache1 = new NamespaceCache();
        NamespaceCache namespaceCache2 = new NamespaceCache();
        string str = (o & LoadOptions.SetBaseUri) != LoadOptions.None ? r.BaseURI : (string)null;
        IXmlLineInfo xmlLineInfo = (o & LoadOptions.SetLineInfo) != LoadOptions.None ? r as IXmlLineInfo : (IXmlLineInfo)null;
        do
        {
          string baseUri = r.BaseURI;
          switch (r.NodeType)
          {
            case XmlNodeType.Element:
            XElement xelement1 = new XElement(namespaceCache1.Get(r.NamespaceURI).GetName(r.LocalName));
            if (str != null && str != baseUri)
              xelement1.SetBaseUri(baseUri);
            if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
              xelement1.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
            if (r.MoveToFirstAttribute())
            {
              do
              {
                XAttribute a = new XAttribute(namespaceCache2.Get(r.Prefix.Length == 0 ? string.Empty : r.NamespaceURI).GetName(r.LocalName), (object)r.Value);
                if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
                  a.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
                xelement1.AppendAttributeSkipNotify(a);
              }
              while (r.MoveToNextAttribute());
              r.MoveToElement();
            }
            xcontainer.AddNodeSkipNotify((XNode)xelement1);
            if (!r.IsEmptyElement)
            {
              xcontainer = (XContainer)xelement1;
              if (str != null)
              {
                str = baseUri;
                goto case XmlNodeType.EndEntity;
              }
              else
                goto case XmlNodeType.EndEntity;
            }
            else
              goto case XmlNodeType.EndEntity;
            case XmlNodeType.Text:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
            if (str != null && str != baseUri || xmlLineInfo != null && xmlLineInfo.HasLineInfo())
            {
              n = (XNode)new XText(r.Value);
              goto case XmlNodeType.EndEntity;
            }
            else
            {
              xcontainer.AddStringSkipNotify(r.Value);
              goto case XmlNodeType.EndEntity;
            }
            case XmlNodeType.CDATA:
            n = (XNode)new XCData(r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.EntityReference:
            if (!r.CanResolveEntity)
              throw new InvalidOperationException(Res.GetString("InvalidOperation_UnresolvedEntityReference"));
            r.ResolveEntity();
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.ProcessingInstruction:
            n = (XNode)new XProcessingInstruction(r.Name, r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.Comment:
            n = (XNode)new XComment(r.Value);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.DocumentType:
            n = (XNode)new XDocumentType(r.LocalName, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value, r.DtdInfo);
            goto case XmlNodeType.EndEntity;
            case XmlNodeType.EndElement:
            if (xcontainer.content == null)
              xcontainer.content = (object)string.Empty;
            XElement xelement2 = xcontainer as XElement;
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
              n = (XNode)null;
            }
            continue;
            default:
            throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedNodeType", new object[1]
              {
                (object) r.NodeType
              }));
          }
        }
        while (r.Read());
      }
    }

    internal void RemoveNode(XNode n)
    {
      bool flag = this.NotifyChanging((object)n, XObjectChangeEventArgs.Remove);
      if (n.parent != this)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
      XNode xnode = (XNode)this.content;
      while (xnode.next != n)
        xnode = xnode.next;
      if (xnode == n)
      {
        this.content = (object)null;
      }
      else
      {
        if (this.content == n)
          this.content = (object)xnode;
        xnode.next = n.next;
      }
      n.parent = (XContainer)null;
      n.next = (XNode)null;
      if (!flag)
        return;
      this.NotifyChanged((object)n, XObjectChangeEventArgs.Remove);
    }

    private void RemoveNodesSkipNotify()
    {
      XNode xnode1 = this.content as XNode;
      if (xnode1 != null)
      {
        do
        {
          XNode xnode2 = xnode1.next;
          xnode1.parent = (XContainer)null;
          xnode1.next = (XNode)null;
          xnode1 = xnode2;
        }
        while (xnode1 != this.content);
      }
      this.content = (object)null;
    }

    internal virtual void ValidateNode(XNode node, XNode previous)
    {
    }

    internal virtual void ValidateString(string s)
    {
    }

    internal void WriteContentTo(XmlWriter writer)
    {
      if (this.content == null)
        return;
      if (this.content is string)
      {
        if (this is XDocument)
          writer.WriteWhitespace((string)this.content);
        else
          writer.WriteString((string)this.content);
      }
      else
      {
        XNode xnode = (XNode)this.content;
        do
        {
          xnode = xnode.next;
          xnode.WriteTo(writer);
        }
        while (xnode != this.content);
      }
    }

    private static void AddContentToList(List<object> list, object content)
    {
      IEnumerable enumerable = content is string ? (IEnumerable)null : content as IEnumerable;
      if (enumerable == null)
      {
        list.Add(content);
      }
      else
      {
        foreach (object content1 in enumerable)
        {
          if (content1 != null)
            XContainer.AddContentToList(list, content1);
        }
      }
    }

    internal static object GetContentSnapshot(object content)
    {
      if (content is string || !(content is IEnumerable))
        return content;
      List<object> list = new List<object>();
      XContainer.AddContentToList(list, content);
      return (object)list;
    }
  }
}
