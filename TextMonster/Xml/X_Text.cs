using System;
using System.Text;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Textknoten dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public class X_Text : X_Node
  {
    internal string text;

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XText"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.Text"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Text;
      }
    }

    /// <summary>
    /// Ruft den Wert des Knotens ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Wert dieses Knotens enthält.
    /// </returns>
    public string Value
    {
      get
      {
        return text;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        text = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XText"/>-Klasse.
    /// </summary>
    /// <param name="value">Der <see cref="T:System.String"/>, der den Wert des <see cref="T:System.Xml.Linq.XText"/>-Knotens enthält.</param>
    public X_Text(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      text = value;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XText"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XText"/>-Objekt.
    /// </summary>
    /// <param name="other">Der <see cref="T:System.Xml.Linq.XText"/>-Knoten, aus dem kopiert werden soll.</param>
    public X_Text(X_Text other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      text = other.text;
    }

    internal X_Text(XmlReader r)
    {
      text = r.Value;
      r.Read();
    }

    /// <summary>
    /// Schreibt diesen Knoten in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      if (parent is X_Document)
        writer.WriteWhitespace(text);
      else
        writer.WriteString(text);
    }

    internal override void AppendText(StringBuilder sb)
    {
      sb.Append(text);
    }

    internal override X_Node CloneNode()
    {
      return new X_Text(this);
    }

    internal override bool DeepEquals(X_Node node)
    {
      if (node != null && NodeType == node.NodeType)
        return text == ((X_Text)node).text;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return text.GetHashCode();
    }
  }
}
