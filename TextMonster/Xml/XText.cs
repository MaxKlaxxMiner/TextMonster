using System;
using System.Text;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Textknoten dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public class XText : XNode
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
        return this.text;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
        this.text = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XText"/>-Klasse.
    /// </summary>
    /// <param name="value">Der <see cref="T:System.String"/>, der den Wert des <see cref="T:System.Xml.Linq.XText"/>-Knotens enthält.</param>
    public XText(string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      this.text = value;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XText"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XText"/>-Objekt.
    /// </summary>
    /// <param name="other">Der <see cref="T:System.Xml.Linq.XText"/>-Knoten, aus dem kopiert werden soll.</param>
    public XText(XText other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      this.text = other.text;
    }

    internal XText(XmlReader r)
    {
      this.text = r.Value;
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
      if (this.parent is XDocument)
        writer.WriteWhitespace(this.text);
      else
        writer.WriteString(this.text);
    }

    internal override void AppendText(StringBuilder sb)
    {
      sb.Append(this.text);
    }

    internal override XNode CloneNode()
    {
      return (XNode)new XText(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      if (node != null && this.NodeType == node.NodeType)
        return this.text == ((XText)node).text;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return this.text.GetHashCode();
    }
  }
}
