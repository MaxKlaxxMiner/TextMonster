using System;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine XML-Verarbeitungsanweisung dar.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class X_ProcessingInstruction : X_Node
  {
    string target;
    string data;

    /// <summary>
    /// Ruft den Zeichenfolgenwert der Verarbeitungsanweisung ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Zeichenfolgenwert der Verarbeitungsanweisung enthält.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Der Zeichenfolgen-<paramref name="value"/> ist null.</exception>
    public string Data
    {
      get
      {
        return data;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        data = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XProcessingInstruction"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.ProcessingInstruction"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.ProcessingInstruction;
      }
    }

    /// <summary>
    /// Ruft eine Zeichenfolge ab, die die Zielanwendung für diese Verarbeitungsanweisung enthält, oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die Zielanwendung für diese Verarbeitungsanweisung enthält.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">Der Zeichenfolgen-<paramref name="value"/> ist null.</exception><exception cref="T:System.ArgumentException">Das Target entspricht nicht den Einschränkungen für XML-Namen.</exception>
    public string Target
    {
      get
      {
        return target;
      }
      set
      {
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Name);
        target = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Name);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XProcessingInstruction"/>-Klasse.
    /// </summary>
    /// <param name="target">Ein <see cref="T:System.String"/> mit der Zielanwendung für diese <see cref="T:System.Xml.Linq.XProcessingInstruction"/>.</param><param name="data">Die Zeichenfolgendaten für diese <see cref="T:System.Xml.Linq.XProcessingInstruction"/>.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="target"/>-Parameter oder der <paramref name="data"/>-Parameter ist null.</exception><exception cref="T:System.ArgumentException">Das <paramref name="target"/> entspricht nicht den Einschränkungen für XML-Namen.</exception>
    public X_ProcessingInstruction(string target, string data)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      this.target = target;
      this.data = data;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XProcessingInstruction"/>-Klasse.
    /// </summary>
    /// <param name="other">Der <see cref="T:System.Xml.Linq.XProcessingInstruction"/>-Knoten, aus dem kopiert werden soll.</param>
    public X_ProcessingInstruction(X_ProcessingInstruction other)
    {
      if (other == null)
        throw new ArgumentNullException("other");
      target = other.target;
      data = other.data;
    }

    internal X_ProcessingInstruction(XmlReader r)
    {
      target = r.Name;
      data = r.Value;
      r.Read();
    }

    /// <summary>
    /// Schreibt diese Verarbeitungsanweisung in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Der <see cref="T:System.Xml.XmlWriter"/>, in den diese Verarbeitungsanweisung geschrieben werden soll.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteProcessingInstruction(target, data);
    }

    internal override X_Node CloneNode()
    {
      return new X_ProcessingInstruction(this);
    }

    internal override bool DeepEquals(X_Node node)
    {
      var xprocessingInstruction = node as X_ProcessingInstruction;
      if (xprocessingInstruction != null && target == xprocessingInstruction.target)
        return data == xprocessingInstruction.data;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return target.GetHashCode() ^ data.GetHashCode();
    }
  }
}
