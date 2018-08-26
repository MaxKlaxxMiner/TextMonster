using System;
using System.Text;
using System.Xml;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine XML-Deklaration dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public class XDeclaration
  {
    /// <summary>
    /// Ruft die Codierung für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Codepagenamen für dieses Dokument enthält.
    /// </returns>
    public string Encoding { get; set; }

    /// <summary>
    /// Ruft die Eigenständigkeitseigenschaft für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die Eigenständigkeitseigenschaft für dieses Dokument enthält.
    /// </returns>
    public string Standalone { get; set; }

    /// <summary>
    /// Ruft die Versionseigenschaft für das Dokument ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die Versionseigenschaft für dieses Dokument enthält.
    /// </returns>
    public string Version { get; set; }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDeclaration"/>-Klasse mit der angegebenen Version, der angegebenen Codierung und dem angegebenen Eigenständigkeitsstatus.
    /// </summary>
    /// <param name="version">Die XML-Version, normalerweise "1.0."</param><param name="encoding">Die Codierung für das XML-Dokument.</param><param name="standalone">Eine Zeichenfolge mit "yes" oder "no", die angibt, ob es sich um eigenständiges XML handelt oder ob externe Entitäten aufgelöst werden müssen.</param>
    public XDeclaration(string version, string encoding, string standalone)
    {
      Version = version;
      Encoding = encoding;
      Standalone = standalone;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XDeclaration"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XDeclaration"/>-Objekt.
    /// </summary>
    /// <param name="other">Die zum Initialisieren dieses <see cref="T:System.Xml.Linq.XDeclaration"/>-Objekts verwendete <see cref="T:System.Xml.Linq.XDeclaration"/>.</param>
    public XDeclaration(XDeclaration other)
    {
      if (other == null) throw new ArgumentNullException("other");
      Version = other.Version;
      Encoding = other.Encoding;
      Standalone = other.Standalone;
    }

    internal XDeclaration(XmlReader r)
    {
      Version = r.GetAttribute("version");
      Encoding = r.GetAttribute("encoding");
      Standalone = r.GetAttribute("standalone");
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
      var stringBuilder = new StringBuilder("<?xml");
      if (Version != null)
      {
        stringBuilder.Append(" version=\"");
        stringBuilder.Append(Version);
        stringBuilder.Append("\"");
      }
      if (Encoding != null)
      {
        stringBuilder.Append(" encoding=\"");
        stringBuilder.Append(Encoding);
        stringBuilder.Append("\"");
      }
      if (Standalone != null)
      {
        stringBuilder.Append(" standalone=\"");
        stringBuilder.Append(Standalone);
        stringBuilder.Append("\"");
      }
      stringBuilder.Append("?>");
      return stringBuilder.ToString();
    }
  }
}
