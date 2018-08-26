using System;
using System.Collections;

namespace TextMonster.Xml
{
  /// <summary>
  /// Enthält Funktionen zum Vergleichen der Dokumentreihenfolge von Knoten. Diese Klasse kann nicht vererbt werden.
  /// </summary>
  public sealed class XNodeDocumentOrderComparer : IComparer, IComparer<XNode>
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XNodeDocumentOrderComparer"/>-Klasse.
    /// </summary>
    public XNodeDocumentOrderComparer()
    {
    }

    /// <summary>
    /// Vergleicht zwei Knoten, um ihre relative Dokumentreihenfolge zu bestimmen.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/> mit dem Wert 0 (null), wenn die Knoten gleich sind, -1, wenn <paramref name="x"/> vor <paramref name="y"/> angeordnet ist, und 1, wenn <paramref name="x"/> nach <paramref name="y"/> angeordnet ist.
    /// </returns>
    /// <param name="x">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><param name="y">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><exception cref="T:System.InvalidOperationException">Die beiden Knoten verfügen über kein gemeinsames übergeordnetes Element.</exception>
    public int Compare(XNode x, XNode y)
    {
      return XNode.CompareDocumentOrder(x, y);
    }

    int IComparer.Compare(object x, object y)
    {
      XNode x1 = x as XNode;
      if (x1 == null && x != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      XNode y1 = y as XNode;
      if (y1 == null && y != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      return this.Compare(x1, y1);
    }
  }
}
