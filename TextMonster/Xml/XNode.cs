using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt das abstrakte Konzept eines Knotens (Element-, Kommentar-, Dokumenttyp-, Verarbeitungsanweisungs- oder Textknoten) in der XML-Struktur dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public abstract class XNode : XObject
  {
    static XNodeDocumentOrderComparer documentOrderComparer;
    static XNodeEqualityComparer equalityComparer;
    internal XNode next;

    /// <summary>
    /// Ruft den nächsten nebengeordneten Knoten dieses Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="T:System.Xml.Linq.XNode"/>, der den nächsten nebengeordneten Knoten enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XNode NextNode
    {
      get
      {
        if (parent != null && this != parent.content)
          return next;
        return null;
      }
    }

    /// <summary>
    /// Ruft den vorherigen nebengeordneten Knoten dieses Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="T:System.Xml.Linq.XNode"/>, der den vorherigen nebengeordneten Knoten enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XNode PreviousNode
    {
      get
      {
        if (parent == null)
          return null;
        var xnode1 = ((XNode)parent.content).next;
        var xnode2 = (XNode)null;
        for (; xnode1 != this; xnode1 = xnode1.next)
          xnode2 = xnode1;
        return xnode2;
      }
    }

    /// <summary>
    /// Ruft einen Vergleich ab, der die relative Position von zwei Knoten vergleichen kann.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNodeDocumentOrderComparer"/>, der die relative Position zweier Knoten vergleichen kann.
    /// </returns>
    public static XNodeDocumentOrderComparer DocumentOrderComparer
    {
      get { return documentOrderComparer ?? (documentOrderComparer = new XNodeDocumentOrderComparer()); }
    }

    /// <summary>
    /// Ruft einen Vergleich ab, der zwei Knoten auf Wertgleichheit vergleichen kann.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNodeEqualityComparer"/>, der zwei Knoten auf Wertgleichheit vergleichen kann.
    /// </returns>
    public static XNodeEqualityComparer EqualityComparer
    {
      get { return equalityComparer ?? (equalityComparer = new XNodeEqualityComparer()); }
    }

    internal XNode()
    {
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt unmittelbar hinter diesem Knoten hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die hinter diesem Knoten hinzugefügt werden sollen.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void AddAfterSelf(object content)
    {
      if (parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      new Inserter(parent, this).Add(content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt unmittelbar hinter diesem Knoten hinzu.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void AddAfterSelf(params object[] content)
    {
      AddAfterSelf((object)content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt direkt vor diesem Knoten hinzu.
    /// </summary>
    /// <param name="content">Ein Inhaltsobjekt, das einfache Inhalte oder eine Auflistung von Inhaltsobjekten enthält, die vor diesem Knoten hinzugefügt werden sollen.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void AddBeforeSelf(object content)
    {
      if (parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      var anchor = (XNode)parent.content;
      while (anchor.next != this)
        anchor = anchor.next;
      if (anchor == parent.content)
        anchor = null;
      new Inserter(parent, anchor).Add(content);
    }

    /// <summary>
    /// Fügt den angegebenen Inhalt direkt vor diesem Knoten hinzu.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten.</param><exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void AddBeforeSelf(params object[] content)
    {
      AddBeforeSelf((object)content);
    }

    /// <summary>
    /// Gibt eine Auflistung der übergeordneten Elemente dieses Knotens zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der übergeordneten Elemente dieses Knotens.
    /// </returns>
    public IEnumerable<XElement> Ancestors()
    {
      return GetAncestors(null, false);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der übergeordneten Elemente dieses Knotens zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der übergeordneten Elemente dieses Knotens. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten. Die Knoten in der zurückgegebenen Auflistung sind in der umgekehrten Dokumentreihenfolge angeordnet. Diese Methode verwendet verzögerte Ausführung.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XElement> Ancestors(XName name)
    {
      if (!(name != null))
        return XElement.EmptySequence;
      return GetAncestors(name, false);
    }

    /// <summary>
    /// Vergleicht zwei Knoten, um ihre relative XML-Dokument-Reihenfolge zu bestimmen.
    /// </summary>
    /// 
    /// <returns>
    /// Ein int mit dem Wert 0 (null), wenn die Knoten gleich sind, -1, wenn <paramref name="n1"/> vor <paramref name="n2"/> angeordnet ist, und 1, wenn <paramref name="n1"/> nach <paramref name="n2"/> angeordnet ist.
    /// </returns>
    /// <param name="n1">Der erste zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><param name="n2">Der zweite zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><exception cref="T:System.InvalidOperationException">Die beiden Knoten verfügen über kein gemeinsames übergeordnetes Element.</exception>
    public static int CompareDocumentOrder(XNode n1, XNode n2)
    {
      if (n1 == n2)
        return 0;
      if (n1 == null)
        return -1;
      if (n2 == null)
        return 1;
      if (n1.parent != n2.parent)
      {
        int num = 0;
        var xnode1 = n1;
        while (xnode1.parent != null)
        {
          xnode1 = xnode1.parent;
          ++num;
        }
        var xnode2 = n2;
        while (xnode2.parent != null)
        {
          xnode2 = xnode2.parent;
          --num;
        }
        if (xnode1 != xnode2)
          throw new InvalidOperationException("InvalidOperation_MissingAncestor");
        if (num < 0)
        {
          do
          {
            n2 = n2.parent;
            ++num;
          }
          while (num != 0);
          if (n1 == n2)
            return -1;
        }
        else if (num > 0)
        {
          do
          {
            n1 = n1.parent;
            --num;
          }
          while (num != 0);
          if (n1 == n2)
            return 1;
        }
        for (; n1.parent != n2.parent; n2 = (XNode)n2.parent)
          n1 = n1.parent;
      }
      else if (n1.parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingAncestor");
      var xnode = (XNode)n1.parent.content;
      do
      {
        xnode = xnode.next;
        if (xnode == n1)
          return -1;
      }
      while (xnode != n2);
      return 1;
    }

    /// <summary>
    /// Erstellt einen <see cref="T:System.Xml.XmlReader"/> für diesen Knoten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.XmlReader"/>, der zum Lesen dieses Knotens und seiner Nachfolgerelemente verwendet werden kann.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public XmlReader CreateReader()
    {
      return new XNodeReader(this, null);
    }

    /// <summary>
    /// Erstellt einen <see cref="T:System.Xml.XmlReader"/> mit den im <paramref name="readerOptions"/>-Parameter angegebenen Optionen.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.XmlReader"/>-Objekt.
    /// </returns>
    /// <param name="readerOptions">Ein <see cref="T:System.Xml.Linq.ReaderOptions"/>-Objekt, das angibt, ob doppelte Namespaces ausgelassen werden sollen.</param>
    public XmlReader CreateReader(ReaderOptions readerOptions)
    {
      return new XNodeReader(this, null, readerOptions);
    }

    /// <summary>
    /// Gibt eine Auflistung der nebengeordneten Knoten nach diesem Knoten in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/> der nebengeordneten Knoten nach diesem Knoten in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XNode> NodesAfterSelf()
    {
      var n = this;
      while (n.parent != null && n != n.parent.content)
      {
        n = n.next;
        yield return n;
      }
    }

    /// <summary>
    /// Gibt eine Auflistung der nebengeordneten Knoten vor diesem Knoten in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XNode"/> der nebengeordneten Knoten vor diesem Knoten in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XNode> NodesBeforeSelf()
    {
      if (parent != null)
      {
        var n = (XNode)parent.content;
        do
        {
          n = n.next;
          if (n != this)
            yield return n;
          else
            break;
        }
        while (parent != null && parent == n.parent);
        n = null;
      }
    }

    /// <summary>
    /// Gibt eine Auflistung der nebengeordneten Elemente nach diesem Knoten in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der nebengeordneten Elemente nach diesem Knoten in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XElement> ElementsAfterSelf()
    {
      return GetElementsAfterSelf(null);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der nebengeordneten Elemente nach diesem Knoten in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der nebengeordneten Elemente nach diesem Knoten in Dokumentreihenfolge. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XElement> ElementsAfterSelf(XName name)
    {
      if (!(name != null))
        return XElement.EmptySequence;
      return GetElementsAfterSelf(name);
    }

    /// <summary>
    /// Gibt eine Auflistung der nebengeordneten Elemente vor diesem Knoten in Dokumentreihenfolge zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der nebengeordneten Elemente vor diesem Knoten in Dokumentreihenfolge.
    /// </returns>
    public IEnumerable<XElement> ElementsBeforeSelf()
    {
      return GetElementsBeforeSelf(null);
    }

    /// <summary>
    /// Gibt eine gefilterte Auflistung der nebengeordneten Elemente vor diesem Knoten in Dokumentreihenfolge zurück. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Xml.Linq.XElement"/> der nebengeordneten Elemente vor diesem Knoten in Dokumentreihenfolge. Nur Elemente, die über einen übereinstimmenden <see cref="T:System.Xml.Linq.XName"/> verfügen, sind in der Auflistung enthalten.
    /// </returns>
    /// <param name="name">Der <see cref="T:System.Xml.Linq.XName"/>, mit dem eine Übereinstimmung gefunden werden soll.</param>
    public IEnumerable<XElement> ElementsBeforeSelf(XName name)
    {
      if (!(name != null))
        return XElement.EmptySequence;
      return GetElementsBeforeSelf(name);
    }

    /// <summary>
    /// Bestimmt, ob der aktuelle Knoten nach einem angegebenen Knoten in der Dokumentreihenfolge angezeigt wird.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieser Knoten nach dem angegebenen Knoten angezeigt wird, andernfalls false.
    /// </returns>
    /// <param name="node">Der <see cref="T:System.Xml.Linq.XNode"/>, dessen Dokumentreihenfolge verglichen werden soll.</param>
    public bool IsAfter(XNode node)
    {
      return CompareDocumentOrder(this, node) > 0;
    }

    /// <summary>
    /// Bestimmt, ob der aktuelle Knoten vor einem angegebenen Knoten in der Dokumentreihenfolge angezeigt wird.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn dieser Knoten vor dem angegebenen Knoten angezeigt wird, andernfalls false.
    /// </returns>
    /// <param name="node">Der <see cref="T:System.Xml.Linq.XNode"/>, dessen Dokumentreihenfolge verglichen werden soll.</param>
    public bool IsBefore(XNode node)
    {
      return CompareDocumentOrder(this, node) < 0;
    }

    /// <summary>
    /// Erstellt einen <see cref="T:System.Xml.Linq.XNode"/> aus einem <see cref="T:System.Xml.XmlReader"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XNode"/>, der den Knoten und dessen Nachfolgerknoten enthält, die vom Reader gelesen wurden. Der Laufzeittyp des Knotens wird vom Knotentyp (<see cref="P:System.Xml.Linq.XObject.NodeType"/>) des ersten im Reader gefundenen Knotens bestimmt.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/> an dem Knoten, der in diesen <see cref="T:System.Xml.Linq.XNode"/> gelesen werden soll.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> ist an keinem bekannten Knotentyp positioniert.</exception><exception cref="T:System.Xml.XmlException">Der zugrunde liegende <see cref="T:System.Xml.XmlReader"/> löst eine Ausnahme aus.</exception><filterpriority>2</filterpriority>
    public static XNode ReadFrom(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.ReadState != ReadState.Interactive)
        throw new InvalidOperationException("InvalidOperation_ExpectedInteractive");
      switch (reader.NodeType)
      {
        case XmlNodeType.Element:
        return new XElement(reader);
        case XmlNodeType.Text:
        case XmlNodeType.Whitespace:
        case XmlNodeType.SignificantWhitespace:
        return new XText(reader);
        case XmlNodeType.CDATA:
        return new XcData(reader);
        case XmlNodeType.ProcessingInstruction:
        return new XProcessingInstruction(reader);
        case XmlNodeType.Comment:
        return new XComment(reader);
        case XmlNodeType.DocumentType:
        return new XDocumentType(reader);
        default:
        throw new InvalidOperationException("InvalidOperation_UnexpectedNodeType");
      }
    }

    /// <summary>
    /// Entfernt diesen Knoten aus seinem übergeordneten Element.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Das übergeordnete Element ist null.</exception>
    public void Remove()
    {
      if (parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      parent.RemoveNode(this);
    }

    /// <summary>
    /// Ersetzt diesen Knoten durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Inhalt, durch den dieser Knoten ersetzt wird.</param>
    public void ReplaceWith(object content)
    {
      if (this.parent == null)
        throw new InvalidOperationException("InvalidOperation_MissingParent");
      var parent = this.parent;
      var anchor = (XNode)this.parent.content;
      while (anchor.next != this)
        anchor = anchor.next;
      if (anchor == this.parent.content)
        anchor = null;
      this.parent.RemoveNode(this);
      if (anchor != null && anchor.parent != parent)
        throw new InvalidOperationException("InvalidOperation_ExternalCode");
      new Inserter(parent, anchor).Add(content);
    }

    /// <summary>
    /// Ersetzt diesen Knoten durch den angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste des neuen Inhalts.</param>
    public void ReplaceWith(params object[] content)
    {
      ReplaceWith((object)content);
    }

    /// <summary>
    /// Gibt das eingezogene XML für diesen Knoten zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der das eingezogene XML enthält.
    /// </returns>
    public override string ToString()
    {
      return GetXmlString(GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Gibt das XML für diesen Knoten zurück, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/> mit dem XML.
    /// </returns>
    /// <param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public string ToString(SaveOptions options)
    {
      return GetXmlString(options);
    }

    /// <summary>
    /// Vergleicht die Werte von zwei Knoten, einschließlich der Werte aller Nachfolgerknoten.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn die Knoten gleich sind, andernfalls false.
    /// </returns>
    /// <param name="n1">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><param name="n2">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param>
    public static bool DeepEquals(XNode n1, XNode n2)
    {
      if (n1 == n2)
        return true;
      if (n1 == null || n2 == null)
        return false;
      return n1.DeepEquals(n2);
    }

    /// <summary>
    /// Schreibt diesen Knoten in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public abstract void WriteTo(XmlWriter writer);

    internal virtual void AppendText(StringBuilder sb)
    {
    }

    internal abstract XNode CloneNode();

    internal abstract bool DeepEquals(XNode node);

    internal IEnumerable<XElement> GetAncestors(XName name, bool self)
    {
      for (var e = (self ? this : (XNode)parent) as XElement; e != null; e = e.parent as XElement)
      {
        if (name == null || e.name == name)
          yield return e;
      }
    }

    IEnumerable<XElement> GetElementsAfterSelf(XName name)
    {
      var n = this;
      while (n.parent != null && n != n.parent.content)
      {
        n = n.next;
        var xelement = n as XElement;
        if (xelement != null && (name == null || xelement.name == name))
          yield return xelement;
      }
    }

    IEnumerable<XElement> GetElementsBeforeSelf(XName name)
    {
      if (parent != null)
      {
        var n = (XNode)parent.content;
        do
        {
          n = n.next;
          if (n != this)
          {
            var xelement = n as XElement;
            if (xelement != null && (name == null || xelement.name == name))
              yield return xelement;
          }
          else
            break;
        }
        while (parent != null && parent == n.parent);
        n = null;
      }
    }

    internal abstract int GetDeepHashCode();

    internal static XmlReaderSettings GetXmlReaderSettings(LoadOptions o)
    {
      var xmlReaderSettings = new XmlReaderSettings();
      if ((o & LoadOptions.PreserveWhitespace) == LoadOptions.None)
        xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
      xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
      xmlReaderSettings.XmlResolver = null;
      return xmlReaderSettings;
    }

    internal static XmlWriterSettings GetXmlWriterSettings(SaveOptions o)
    {
      var xmlWriterSettings = new XmlWriterSettings();
      if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
        xmlWriterSettings.Indent = true;
      if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
        xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
      return xmlWriterSettings;
    }

    string GetXmlString(SaveOptions o)
    {
      using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
      {
        var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
          settings.Indent = true;
        if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
          settings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
        if (this is XText)
          settings.ConformanceLevel = ConformanceLevel.Fragment;
        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
          var xdocument = this as XDocument;
          if (xdocument != null)
            xdocument.WriteContentTo(writer);
          else
            WriteTo(writer);
        }
        return stringWriter.ToString();
      }
    }
  }
}
