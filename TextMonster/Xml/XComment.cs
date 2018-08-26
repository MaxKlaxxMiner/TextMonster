using System;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen XML-Kommentar dar.
  /// </summary>
  public class XComment : XNode
  {
    string value;

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XComment"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.Comment"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Comment;
      }
    }

    /// <summary>
    /// Ruft den Zeichenfolgenwert des Kommentars ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/> mit dem Zeichenfolgenwert dieses Kommentars.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Die <paramref name="value"/> ist null.</exception>
    public string Value
    {
      get
      {
        return value;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        bool flag = NotifyChanging(this, XObjectChangeEventArgs.Value);
        this.value = value;
        if (!flag)
          return;
        NotifyChanged(this, XObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XComment"/>-Klasse mit dem angegebenen Zeichenfolgeninhalt.
    /// </summary>
    /// <param name="value">Eine Zeichenfolge, die den Inhalt des neuen <see cref="T:System.Xml.Linq.XComment"/>-Objekts enthält.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="value"/>-Parameter ist null.</exception>
    public XComment(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.value = value;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XComment"/>-Klasse mit einem vorhandenen Kommentarknoten.
    /// </summary>
    /// <param name="other">Der <see cref="T:System.Xml.Linq.XComment"/>-Knoten, aus dem kopiert werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="other"/>-Parameter ist null.</exception>
    public XComment(XComment other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      value = other.value;
    }

    internal XComment(XmlReader r)
    {
      value = r.Value;
      r.Read();
    }

    /// <summary>
    /// Schreibt diesen Kommentar in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteComment(value);
    }

    internal override XNode CloneNode()
    {
      return new XComment(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      var xcomment = node as XComment;
      if (xcomment != null)
        return value == xcomment.value;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return value.GetHashCode();
    }
  }
}
