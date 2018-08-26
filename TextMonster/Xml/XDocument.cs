using System;
using System.IO;
using System.Text;
using System.Xml;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt ein XML-Dokument dar.
  /// </summary>
  public class XDocument : XContainer
  {
    /// <summary>
    /// Ruft die XML-Deklaration für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.Xml.Linq.XDeclaration"/>, die die XML-Deklaration für dieses Dokument enthält.
    /// </returns>
    public XDeclaration Declaration { get; set; }

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
        return GetFirstNode<XDocumentType>();
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
        return GetFirstNode<XElement>();
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
      AddContentSkipNotify(content);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse mit der angegebenen <see cref="T:System.Xml.Linq.XDeclaration"/> und dem angegebenen Inhalt.
    /// </summary>
    /// <param name="declaration">Eine <see cref="T:System.Xml.Linq.XDeclaration"/> für das Dokument.</param><param name="content">Der Inhalt des Dokuments.</param>
    public XDocument(XDeclaration declaration, params object[] content)
      : this(content)
    {
      Declaration = declaration;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDocument"/>-Klasse mit einem vorhandenen <see cref="T:System.Xml.Linq.XDocument"/>-Objekt.
    /// </summary>
    /// <param name="other">Das <see cref="T:System.Xml.Linq.XDocument"/>-Objekt, das kopiert wird.</param>
    public XDocument(XDocument other)
      : base(other)
    {
      if (other.Declaration == null)
        return;
      Declaration = new XDeclaration(other.Declaration);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Datei, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/> mit dem Inhalt der angegebenen Datei.
    /// </returns>
    /// <param name="uri">Eine URI-Zeichenfolge, die auf die Datei verweist, die in ein neues <see cref="T:System.Xml.Linq.XDocument"/> geladen werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(string uri, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(uri, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Erstellt mithilfe des angegebenen Streams eine neue <see cref="T:System.Xml.Linq.XDocument"/>-Instanz, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>-Objekt, mit dem die im Stream enthaltenen Daten gelesen werden.
    /// </returns>
    /// <param name="stream">Der Stream, der die XML-Daten enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(Stream stream, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(stream, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einem <see cref="T:System.IO.TextReader"/>, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.IO.TextReader"/> gelesen wurden.
    /// </returns>
    /// <param name="textReader">Ein <see cref="T:System.IO.TextReader"/>, der den Inhalt für das <see cref="T:System.Xml.Linq.XDocument"/> enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(TextReader textReader, LoadOptions options = LoadOptions.None)
    {
      var xmlReaderSettings = GetXmlReaderSettings(options);
      using (var reader = XmlReader.Create(textReader, xmlReaderSettings))
        return Load(reader, options);
    }

    /// <summary>
    /// Lädt ein <see cref="T:System.Xml.Linq.XElement"/> aus einem <see cref="T:System.Xml.XmlReader"/>, wobei optional der Basis-URI festgelegt wird und die Zeileninformationen beibehalten werden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das die XML-Daten enthält, die aus dem angegebenen <see cref="T:System.Xml.XmlReader"/> gelesen wurden.
    /// </returns>
    /// <param name="reader">Ein <see cref="T:System.Xml.XmlReader"/>, dessen Inhalt des <see cref="T:System.Xml.Linq.XDocument"/> gelesen wird.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das angibt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Load(XmlReader reader, LoadOptions options = LoadOptions.None)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.ReadState == ReadState.Initial)
        reader.Read();
      var xdocument = new XDocument();
      if ((options & LoadOptions.SetBaseUri) != LoadOptions.None)
      {
        string baseUri = reader.BaseURI;
        if (!string.IsNullOrEmpty(baseUri))
          xdocument.SetBaseUri(baseUri);
      }
      if ((options & LoadOptions.SetLineInfo) != LoadOptions.None)
      {
        var xmlLineInfo = reader as IXmlLineInfo;
        if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
          xdocument.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
      }
      if (reader.NodeType == XmlNodeType.XmlDeclaration)
        xdocument.Declaration = new XDeclaration(reader);
      xdocument.ReadContentFrom(reader, options);
      if (!reader.EOF)
        throw new InvalidOperationException("InvalidOperation_ExpectedEndOfFile");
      if (xdocument.Root == null)
        throw new InvalidOperationException("InvalidOperation_MissingRoot");
      return xdocument;
    }

    /// <summary>
    /// Erstellt ein neues <see cref="T:System.Xml.Linq.XDocument"/> aus einer Zeichenfolge, wobei optional Leerraum und Zeileninformationen beibehalten werden und der Basis-URI festgelegt wird.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XDocument"/>, das aus der Zeichenfolge aufgefüllt wird, die XML enthält.
    /// </returns>
    /// <param name="text">Eine Zeichenfolge, die XML enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.LoadOptions"/>, das Leerraumverhalten angibt und festlegt, ob Basis-URI- und Zeileninformationen geladen werden.</param>
    public static XDocument Parse(string text, LoadOptions options = LoadOptions.None)
    {
      using (var stringReader = new StringReader(text))
      {
        var xmlReaderSettings = GetXmlReaderSettings(options);
        using (var reader = XmlReader.Create(stringReader, xmlReaderSettings))
          return Load(reader, options);
      }
    }

    /// <summary>
    /// Serialisieren Sie dieses <see cref="T:System.Xml.Linq.XDocument"/> in eine Datei, und überschreiben Sie dabei eine vorhandene Datei, sofern vorhanden.
    /// </summary>
    /// <param name="fileName">Eine Zeichenfolge, die den Namen der Datei enthält.</param>
    public void Save(string fileName)
    {
      Save(fileName, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in eine Datei, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="fileName">Eine Zeichenfolge, die den Namen der Datei enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(string fileName, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      if (Declaration != null)
      {
        if (!string.IsNullOrEmpty(Declaration.Encoding))
        {
          try
          {
            xmlWriterSettings.Encoding = Encoding.GetEncoding(Declaration.Encoding);
          }
          catch (ArgumentException)
          {
          }
        }
      }
      using (var writer = XmlWriter.Create(fileName, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XDocument"/> an den angegebenen <see cref="T:System.IO.Stream"/> aus.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XDocument"/> ausgegeben werden soll.</param>
    public void Save(Stream stream)
    {
      Save(stream, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XDocument"/> zum angegebenen <see cref="T:System.IO.Stream"/> aus und gibt Formatierungsverhalten optional an.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XDocument"/> ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(Stream stream, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      if (Declaration != null)
      {
        if (!string.IsNullOrEmpty(Declaration.Encoding))
        {
          try
          {
            xmlWriterSettings.Encoding = Encoding.GetEncoding(Declaration.Encoding);
          }
          catch (ArgumentException)
          {
          }
        }
      }
      using (var writer = XmlWriter.Create(stream, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.IO.TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">Ein <see cref="T:System.IO.TextWriter"/>, in den das <see cref="T:System.Xml.Linq.XDocument"/> geschrieben wird.</param>
    public void Save(TextWriter textWriter)
    {
      Save(textWriter, GetSaveOptionsFromAnnotations());
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.IO.TextWriter"/>, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="textWriter">Der <see cref="T:System.IO.TextWriter"/>, an den das XML ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(TextWriter textWriter, SaveOptions options)
    {
      var xmlWriterSettings = GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(textWriter, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses <see cref="T:System.Xml.Linq.XDocument"/> in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den das <see cref="T:System.Xml.Linq.XDocument"/> geschrieben wird.</param>
    public void Save(XmlWriter writer)
    {
      WriteTo(writer);
    }

    /// <summary>
    /// Schreibt dieses Dokument in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (Declaration != null && Declaration.Standalone == "yes")
        writer.WriteStartDocument(true);
      else if (Declaration != null && Declaration.Standalone == "no")
        writer.WriteStartDocument(false);
      else
        writer.WriteStartDocument();
      WriteContentTo(writer);
      writer.WriteEndDocument();
    }

    internal override void AddAttribute(XAttribute a)
    {
      throw new ArgumentException("Argument_AddAttribute");
    }

    internal override void AddAttributeSkipNotify(XAttribute a)
    {
      throw new ArgumentException("Argument_AddAttribute");
    }

    internal override XNode CloneNode()
    {
      return new XDocument(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      var xdocument = node as XDocument;
      if (xdocument != null)
        return ContentsEqual(xdocument);
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return ContentsHashCode();
    }

    T GetFirstNode<T>() where T : XNode
    {
      var xnode = content as XNode;
      if (xnode != null)
      {
        do
        {
          xnode = xnode.next;
          var obj = xnode as T;
          if (obj != null)
            return obj;
        }
        while (xnode != content);
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
        ValidateDocument(previous, XmlNodeType.DocumentType, XmlNodeType.None);
        break;
        case XmlNodeType.Text:
        ValidateString(((XText)node).Value);
        break;
        case XmlNodeType.CDATA:
        throw new ArgumentException("Argument_AddNode");
        case XmlNodeType.Document:
        throw new ArgumentException("Argument_AddNode");
        case XmlNodeType.DocumentType:
        ValidateDocument(previous, XmlNodeType.None, XmlNodeType.Element);
        break;
      }
    }

    void ValidateDocument(XNode previous, XmlNodeType allowBefore, XmlNodeType allowAfter)
    {
      var xnode = content as XNode;
      if (xnode == null)
        return;
      if (previous == null)
        allowBefore = allowAfter;
      do
      {
        xnode = xnode.next;
        var nodeType = xnode.NodeType;
        switch (nodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.DocumentType:
          if (nodeType != allowBefore)
            throw new InvalidOperationException("InvalidOperation_DocumentStructure");
          allowBefore = XmlNodeType.None;
          break;
        }
        if (xnode == previous)
          allowBefore = allowAfter;
      }
      while (xnode != content);
    }

    internal override void ValidateString(string s)
    {
      if (!IsWhitespace(s))
        throw new ArgumentException("Argument_AddNonWhitespace");
    }
  }
}
