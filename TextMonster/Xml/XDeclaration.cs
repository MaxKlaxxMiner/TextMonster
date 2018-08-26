using System;
using System.Text;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine XML-Deklaration dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public class XDeclaration
  {
    private string version;
    private string encoding;
    private string standalone;

    /// <summary>
    /// Ruft die Codierung für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Codepagenamen für dieses Dokument enthält.
    /// </returns>
    public string Encoding
    {
      get
      {
        return this.encoding;
      }
      set
      {
        this.encoding = value;
      }
    }

    /// <summary>
    /// Ruft die Eigenständigkeitseigenschaft für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die Eigenständigkeitseigenschaft für dieses Dokument enthält.
    /// </returns>
    public string Standalone
    {
      get
      {
        return this.standalone;
      }
      set
      {
        this.standalone = value;
      }
    }

    /// <summary>
    /// Ruft die Versionseigenschaft für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die Versionseigenschaft für dieses Dokument enthält.
    /// </returns>
    public string Version
    {
      get
      {
        return this.version;
      }
      set
      {
        this.version = value;
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDeclaration"/>-Klasse mit der angegebenen Version, der angegebenen Codierung und dem angegebenen Eigenständigkeitsstatus.
    /// </summary>
    /// <param name="version">Die XML-Version, normalerweise "1.0."</param><param name="encoding">Die Codierung für das XML-Dokument.</param><param name="standalone">Eine Zeichenfolge mit "yes" oder "no", die angibt, ob es sich um eigenständiges XML handelt oder ob externe Entitäten aufgelöst werden müssen.</param>
    public XDeclaration(string version, string encoding, string standalone)
    {
      this.version = version;
      this.encoding = encoding;
      this.standalone = standalone;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDeclaration"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XDeclaration"/>-Objekt.
    /// </summary>
    /// <param name="other">Die zum Initialisieren dieses <see cref="T:System.Xml.Linq.XDeclaration"/>-Objekts verwendete <see cref="T:System.Xml.Linq.XDeclaration"/>.</param>
    public XDeclaration(XDeclaration other)
    {
      if (other == null) throw new ArgumentNullException("other");
      this.version = other.version;
      this.encoding = other.encoding;
      this.standalone = other.standalone;
    }

    internal XDeclaration(XmlReader r)
    {
      this.version = r.GetAttribute("version");
      this.encoding = r.GetAttribute("encoding");
      this.standalone = r.GetAttribute("standalone");
      r.Read();
    }

    /// <summary>
    /// Stellt die Deklaration als formatierte Zeichenfolge bereit.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die formatierte XML-Zeichenfolge enthält.
    /// </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("<?xml");
      if (this.version != null)
      {
        stringBuilder.Append(" version=\"");
        stringBuilder.Append(this.version);
        stringBuilder.Append("\"");
      }
      if (this.encoding != null)
      {
        stringBuilder.Append(" encoding=\"");
        stringBuilder.Append(this.encoding);
        stringBuilder.Append("\"");
      }
      if (this.standalone != null)
      {
        stringBuilder.Append(" standalone=\"");
        stringBuilder.Append(this.standalone);
        stringBuilder.Append("\"");
      }
      stringBuilder.Append("?>");
      return stringBuilder.ToString();
    }
  }
}
