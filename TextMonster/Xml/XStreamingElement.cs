using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt Elemente in einer XML-Struktur dar, die verzögerte Streamingausgabe unterstützt.
  /// </summary>
  public class XStreamingElement
  {
    internal XName name;
    internal object content;

    /// <summary>
    /// Ruft den Namen des Streamingelements ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen dieses Streamingelements enthält.
    /// </returns>
    public XName Name
    {
      get
      {
        return name;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        name = value;
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XElement"/>-Klasse mit dem angegebenen <see cref="T:System.Xml.Linq.XName"/>.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Namen des Elements enthält.</param>
    public XStreamingElement(XName name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      this.name = name;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XStreamingElement"/>-Klasse mit dem angegebenen Namen und Inhalt.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.Xml.Linq.XName"/>, der den Elementnamen enthält.</param><param name="content">Der Inhalt des Elements.</param>
    public XStreamingElement(XName name, params object[] content)
      : this(name)
    {
      this.content = content;
    }

    /// <summary>
    /// Fügt diesem <see cref="T:System.Xml.Linq.XStreamingElement"/> den angegebenen Inhalt als untergeordnetes Element hinzu.
    /// </summary>
    /// <param name="content">Inhalt, der dem Streamingelement hinzugefügt werden soll.</param>
    public void Add(object content)
    {
      if (content == null)
        return;
      var list = this.content as List<object>;
      if (list == null)
      {
        list = new List<object>();
        if (this.content != null)
          list.Add(this.content);
        this.content = list;
      }
      list.Add(content);
    }

    /// <summary>
    /// Fügt diesem <see cref="T:System.Xml.Linq.XStreamingElement"/> den angegebenen Inhalt als untergeordnetes Element hinzu.
    /// </summary>
    /// <param name="content">Inhalt, der dem Streamingelement hinzugefügt werden soll.</param>
    public void Add(params object[] content)
    {
      Add((object)content);
    }

    /// <summary>
    /// Serialisiert dieses Streamingelement in eine Datei, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="fileName">Ein <see cref="T:System.String"/>, der den Namen der Datei enthält.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>-Objekt, das das Formatierungsverhalten angibt.</param>
    public void Save(string fileName, SaveOptions options = SaveOptions.None)
    {
      var xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(fileName, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Gibt dieses <see cref="T:System.Xml.Linq.XStreamingElement"/> zum angegebenen <see cref="T:System.IO.Stream"/> aus und gibt Formatierungsverhalten optional an.
    /// </summary>
    /// <param name="stream">Der Stream, in den dieses <see cref="T:System.Xml.Linq.XDocument"/> ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>-Objekt, das das Formatierungsverhalten angibt.</param>
    public void Save(Stream stream, SaveOptions options = SaveOptions.None)
    {
      var xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(stream, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses Streamingelement in einen <see cref="T:System.IO.TextWriter"/>, wobei optional die Formatierung deaktiviert wird.
    /// </summary>
    /// <param name="textWriter">Der <see cref="T:System.IO.TextWriter"/>, an den das XML ausgegeben werden soll.</param><param name="options">Ein <see cref="T:System.Xml.Linq.SaveOptions"/>, das Formatierungsverhalten angibt.</param>
    public void Save(TextWriter textWriter, SaveOptions options = SaveOptions.None)
    {
      var xmlWriterSettings = XNode.GetXmlWriterSettings(options);
      using (var writer = XmlWriter.Create(textWriter, xmlWriterSettings))
        Save(writer);
    }

    /// <summary>
    /// Serialisiert dieses Streamingelement in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den das <see cref="T:System.Xml.Linq.XElement"/> geschrieben wird.</param>
    public void Save(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteStartDocument();
      WriteTo(writer);
      writer.WriteEndDocument();
    }

    /// <summary>
    /// Gibt das formatierte (eingezogene) XML für dieses Streamingelement zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der das eingezogene XML enthält.
    /// </returns>
    public override string ToString()
    {
      return GetXmlString(SaveOptions.None);
    }

    /// <summary>
    /// Gibt das XML für dieses Streamingelement zurück, wobei optional die Formatierung deaktiviert wird.
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
    /// Schreibt dieses Streamingelement in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      new StreamingElementWriter(writer).WriteStreamingElement(this);
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
        using (var writer = XmlWriter.Create(stringWriter, settings))
          WriteTo(writer);
        return stringWriter.ToString();
      }
    }
  }
}
