using System;
using System.Xml;
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Textknoten dar, der CDATA enthält.
  /// </summary>
  public class XcData : XText
  {
    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XCData"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.CDATA"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.CDATA;
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XCData"/>-Klasse.
    /// </summary>
    /// <param name="value">Eine Zeichenfolge, die den Wert des <see cref="T:System.Xml.Linq.XCData"/>-Knotens enthält.</param>
    public XcData(string value)
      : base(value)
    {
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XCData"/>-Klasse.
    /// </summary>
    /// <param name="other">Der <see cref="T:System.Xml.Linq.XCData"/>-Knoten, aus dem kopiert werden soll.</param>
    public XcData(XcData other)
      : base(other)
    {
    }

    internal XcData(XmlReader r)
      : base(r)
    {
    }

    /// <summary>
    /// Schreibt dieses CDATA-Objekt in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteCData(text);
    }

    internal override XNode CloneNode()
    {
      return new XcData(this);
    }
  }
}
