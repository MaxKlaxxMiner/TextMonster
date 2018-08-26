using System;
using System.Collections;

namespace TextMonster.Xml
{
  /// <summary>
  /// Vergleicht Knoten auf Gleichheit. Diese Klasse kann nicht vererbt werden.
  /// </summary>
  public sealed class XNodeEqualityComparer : IEqualityComparer, IEqualityComparer<XNode>
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XNodeEqualityComparer"/>-Klasse.
    /// </summary>
    public XNodeEqualityComparer()
    {
    }

    /// <summary>
    /// Vergleicht die Werte zweier Knoten.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Boolean"/>, das angibt, ob die Knoten gleich sind.
    /// </returns>
    /// <param name="x">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><param name="y">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param>
    public bool Equals(XNode x, XNode y)
    {
      return XNode.DeepEquals(x, y);
    }

    /// <summary>
    /// Gibt einen Hashcode auf der Grundlage eines <see cref="T:System.Xml.Linq.XNode"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/>, das einen wertbasierten Hashcode für den Knoten enthält.
    /// </returns>
    /// <param name="obj">Der zu hashende <see cref="T:System.Xml.Linq.XNode"/>.</param>
    public int GetHashCode(XNode obj)
    {
      if (obj == null)
        return 0;
      return obj.GetDeepHashCode();
    }

    bool IEqualityComparer.Equals(object x, object y)
    {
      XNode x1 = x as XNode;
      if (x1 == null && x != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      XNode y1 = y as XNode;
      if (y1 == null && y != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      return this.Equals(x1, y1);
    }

    int IEqualityComparer.GetHashCode(object obj)
    {
      XNode xnode = obj as XNode;
      if (xnode == null && obj != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      return this.GetHashCode(xnode);
    }
  }
}
