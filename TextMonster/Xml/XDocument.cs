using System;
using System.IO;
using System.Text;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Dokument dar.
  /// </summary>
  public class XDocument : XContainer
  {
    private XDeclaration declaration;

    /// <summary>
    /// Ruft die XML-Deklaration für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.Xml.Linq.XDeclaration"/>, die die XML-Deklaration für dieses Dokument enthält.
    /// </returns>
    public XDeclaration Declaration
    {
      get
      {
        return this.declaration;
      }
      set
      {
        this.declaration = value;
      }
    }

    /// <summary>
    /// Ruft die Dokumenttypdefinition (DTD) für dieses Dokument ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocumentType"/>, der die DTD für dieses Dokument enthält.
    /// </returns>
    public XDocumentType DocumentType
    {
      get
      {
        return this.GetFirstNode<XDocumentType>();
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XDocument"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.Document"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Document;
      }
    }

    /// <summary>
    /// Ruft das Stammelement der XML-Struktur für dieses Dokument ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das Stamm-<see cref="T:System.Xml.Linq.XElement"/> der XML-Struktur.
    /// </returns>
    public XElement Root
    {
      get
      {
        return this.GetFirstNode<XElement>();
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse.
    /// </summary>
    public XDocument()
    {
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse mit dem angegebenen Inhalt.
    /// </summary>
    /// <param name="content">Eine Parameterliste von Inhaltsobjekten, die diesem Dokument hinzugefügt werden sollen.</param>
    public XDocument(params object[] content)
      : this()
    {
      this.AddContentSkipNotify((object)content);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse mit der angegebenen <see cref="T:System.Xml.Linq.XDeclaration"/> und dem angegebenen Inhalt.
    /// </summary>
    /// <param name="declaration">Eine <see cref="T:System.Xml.Linq.XDeclaration"/> für das Dokument.</param><param name="content">Der Inhalt des Dokuments.</param>
    public XDocument(XDeclaration declaration, params object[] content)
      : this(content)
    {
      this.declaration = declaration;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse mit einem vorhandenen <see cref="T:System.Xml.Linq.XDocument"/>-Objekt.
    /// </summary>
    /// <param name="other">Das <see cref="T:System.Xml.Linq.XDocument"/>-Objekt, das kopiert wird.</param>
    public XDocument(XDocument other)
      : base((XContainer)other)
    {
      if (other.declaration == null)
        return;
      this.declaration = new XDeclaration(other.declaration);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Datei.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein neues <see cref="T:System.Xml.Linq.XDocument"/> geladen werden soll.</param>
    public static XDocument Load(string uri)
    {
      return XDocument.Load(uri, LoadOptions.None);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Datei, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein neues <see cref="T:System.Xml.Linq.XDocument"/> geladen werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(string uri, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(uri, xmlReaderSettings))
        return XDocument.Load(reader, options);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Stream eine neue <see cref="T:System.Xml.Linq.XDocument"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param>
    public static XDocument Load(Stream stream)
    {
      return XDocument.Load(stream, LoadOptions.None);
    }

    /// <summary>
    /// Erstellt mithilfe des angegebenen Streams eine neue <see cref="T:System.Xml.Linq.XDocument"/>-Instanz, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(Stream stream, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(stream, xmlReaderSettings))
        return XDocument.Load(reader, options);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einem <see cref="T:System.IO.TextReader"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/> mit dem Inhalt des angegebenen <see cref="T:System.IO.TextReader"/>.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, der den Inhalt für das <see cref="T:System.Xml.Linq.XDocument"/> enthält.</param>
    public static XDocument Load(TextReader textReader)
    {
      return XDocument.Load(textReader, LoadOptions.None);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einem <see cref="T:System.IO.TextReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.IO.TextReader"/> gelesen wurden.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, der den Inhalt für das <see cref="T:System.Xml.Linq.XDocument"/> enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(TextReader textReader, LoadOptions options)
    {
      XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
      using (XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings))
        return XDocument.Load(reader, options);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einem <see cref="T:System.Xml.XmlReader"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/> mit dem Inhalt des angegebenen <see cref="T:System.Xml.XmlReader"/>.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, der den Inhalt für das <see cref="T:System.Xml.Linq.XDocument"/> enthält.</param>
    public static XDocument Load(XmlReader reader)
    {
      return XDocument.Load(reader, LoadOptions.None);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.Xml.XmlReader"/>, wobei optional der Basis-URI festgelegt wird und die Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.Xml.XmlReader"/> gelesen wurden.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, dessen Inhalt des <see cref="T:System.Xml.Linq.XDocument"/> gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(XmlReader reader, LoadOptions options)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.ReadState == ReadState.Initial)
        reader.Read();
      XDocument xdocument = new XDocument();
      if ((options & LoadOptions.SetBaseUri) != LoadOptions.None)
      {
        string baseUri = reader.BaseURI;
        if (baseUri != null && baseUri.Length != 0)
          xdocument.SetBaseUri(baseUri);
      }
      if ((options & LoadOptions.SetLineInfo) != LoadOptions.None)
      {
        IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
        if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
          xdocument.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
      }
      if (reader.NodeType == XmlNodeType.XmlDeclaration)
        xdocument.Declaration = new XDeclaration(reader);
      xdocument.ReadContentFrom(reader, options);
      if (!reader.EOF)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedEndOfFile"));
      if (xdocument.Root == null)
        throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingRoot"));
      return xdocument;
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Zeichenfolge.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Eine Zeichenfolge, die XML enthält.</param>
    public static XDocument Parse(string text)
    {
      return XDocument.Parse(text, LoadOptions.None);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Zeichenfolge, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Eine Zeichenfolge, die XML enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Parse(string text, LoadOptions options)
    {
      using (StringReader stringReader = new StringReader(text))
      {
        XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
        using (XmlReader reader = XmlReader.Create((TextReader)stringReader, xmlReaderSettings))
          return XDocument.Load(reader, options);
      }
    }

    /// <summary>
    /// Serialisieren Sie dieses <see cref="T:System.Xml.Linq.XDocument"/> in eine Datei, und überschreiben Sie dabei eine vorhandene Datei, sofern vorhanden.
    /// </summary>
    /// <param name="fileName">Eine Zeichenfolge, die den Namen der Datei enthält.</param>
    public void Save(string fileName)
    {
      this.Save(fileName, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in eine Datei, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="fileName">Eine Zeichenfolge, die den Namen der Datei enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(string fileName, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      if (this.declaration != null)
      {
        if (!string.IsNullOrEmpty(this.declaration.Encoding))
        {
          try
          {
            xmlWriterSettings.Encoding = Encoding.GetEncoding(this.declaration.Encoding);
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      using (XmlWriter writer = XmlWriter.Create(fileName, xmlWriterSettings))
        this.Save(writer);
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XDocument"/> an den angegebenen <see cref="T:System.IO.Stream"/> aus.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XDocument"/> ausgegeben werden soll.</param>
    public void Save(Stream stream)
    {
      this.Save(stream, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XDocument"/> zum angegebenen <see cref="T:System.IO.Stream"/> aus und gibt Formatierungsverhalten optional an.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XDocument"/> ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(Stream stream, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      if (this.declaration != null)
      {
        if (!string.IsNullOrEmpty(this.declaration.Encoding))
        {
          try
          {
            xmlWriterSettings.Encoding = Encoding.GetEncoding(this.declaration.Encoding);
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      using (XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings))
        this.Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.IO.TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">Ein <see cref="T:System.IO.TextWriter"/>, in den das <see cref="T:System.Xml.Linq.XDocument"/> geschrieben wird.</param>
    public void Save(TextWriter textWriter)
    {
      this.Save(textWriter, this.GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.IO.TextWriter"/>, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="textWriter">Der <see cref="T:System.IO.TextWriter"/>, an den das XML ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(TextWriter textWriter, SaveOptions options)
    {
      XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (XmlWriter writer = XmlWriter.Create(textWriter, xmlWriterSettings))
        this.Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den das <see cref="T:System.Xml.Linq.XDocument"/> geschrieben wird.</param>
    public void Save(XmlWriter writer)
    {
      this.WriteTo(writer);
    }

    /// <summary>
    /// Schreibt dieses Dokument in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (this.declaration != null && this.declaration.Standalone == "yes")
        writer.WriteStartDocument(true);
      else if (this.declaration != null && this.declaration.Standalone == "no")
        writer.WriteStartDocument(false);
      else
        writer.WriteStartDocument();
      this.WriteContentTo(writer);
      writer.WriteEndDocument();
    }

    internal override void AddAttribute(XAttribute a)
    {
      throw new ArgumentException(Res.GetString("Argument_AddAttribute"));
    }

    internal override void AddAttributeSkipNotify(XAttribute a)
    {
      throw new ArgumentException(Res.GetString("Argument_AddAttribute"));
    }

    internal override XNode CloneNode()
    {
      return (XNode)new XDocument(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      XDocument xdocument = node as XDocument;
      if (xdocument != null)
        return this.ContentsEqual((XContainer)xdocument);
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return this.ContentsHashCode();
    }

    private T GetFirstNode<T>() where T : XNode
    {
      XNode xnode = this.content as XNode;
      if (xnode != null)
      {
        do
        {
          xnode = xnode.next;
          T obj = xnode as T;
          if ((object)obj != null)
            return obj;
        }
        while (xnode != this.content);
      }
      return default(T);
    }

    internal static bool IsWhitespace(string s)
    {
      foreach (char ch in s)
      {
        switch (ch)
        {
          case ' ':
          case '\t':
          case '\r':
          case '\n':
          goto case ' ';
          default:
          return false;
        }
      }
      return true;
    }

    internal override void ValidateNode(XNode node, XNode previous)
    {
      switch (node.NodeType)
      {
        case XmlNodeType.Element:
        this.ValidateDocument(previous, XmlNodeType.DocumentType, XmlNodeType.None);
        break;
        case XmlNodeType.Text:
        this.ValidateString(((XText)node).Value);
        break;
        case XmlNodeType.CDATA:
        throw new ArgumentException(Res.GetString("Argument_AddNode", new object[1]
          {
            (object) XmlNodeType.CDATA
          }));
        case XmlNodeType.Document:
        throw new ArgumentException(Res.GetString("Argument_AddNode", new object[1]
          {
            (object) XmlNodeType.Document
          }));
        case XmlNodeType.DocumentType:
        this.ValidateDocument(previous, XmlNodeType.None, XmlNodeType.Element);
        break;
      }
    }

    private void ValidateDocument(XNode previous, XmlNodeType allowBefore, XmlNodeType allowAfter)
    {
      XNode xnode = this.content as XNode;
      if (xnode == null)
        return;
      if (previous == null)
        allowBefore = allowAfter;
      do
      {
        xnode = xnode.next;
        XmlNodeType nodeType = xnode.NodeType;
        switch (nodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.DocumentType:
          if (nodeType != allowBefore)
            throw new InvalidOperationException(Res.GetString("InvalidOperation_DocumentStructure"));
          allowBefore = XmlNodeType.None;
          break;
        }
        if (xnode == previous)
          allowBefore = allowAfter;
      }
      while (xnode != this.content);
    }

    internal override void ValidateString(string s)
    {
      if (!XDocument.IsWhitespace(s))
        throw new ArgumentException(Res.GetString("Argument_AddNonWhitespace"));
    }
  }
}
