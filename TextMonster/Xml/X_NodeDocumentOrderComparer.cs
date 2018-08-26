using System;
using System.Collections;
using System.Collections.Generic;

namespace TextMonster.Xml
{
  /// <summary>
  /// Enthält Funktionen zum Vergleichen der Dokumentreihenfolge von Knoten. Diese Klasse kann nicht vererbt werden.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public sealed class X_NodeDocumentOrderComparer : IComparer, IComparer<X_Node>
  {
    /// <summary>
    /// Vergleicht zwei Knoten, um ihre relative Dokumentreihenfolge zu bestimmen.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Int32"/> mit dem Wert 0 (null), wenn die Knoten gleich sind, -1, wenn <paramref name="x"/> vor <paramref name="y"/> angeordnet ist, und 1, wenn <paramref name="x"/> nach <paramref name="y"/> angeordnet ist.
    /// </returns>
    /// <param name="x">Das erste zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><param name="y">Das zweite zu vergleichende <see cref="T:System.Xml.Linq.XNode"/>.</param><exception cref="T:System.InvalidOperationException">Die beiden Knoten verfügen über kein gemeinsames übergeordnetes Element.</exception>
    public int Compare(X_Node x, X_Node y)
    {
      return X_Node.CompareDocumentOrder(x, y);
    }

    int IComparer.Compare(object x, object y)
    {
      var x1 = x as X_Node;
      if (x1 == null && x != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      var y1 = y as X_Node;
      if (y1 == null && y != null)
        throw new ArgumentException("Argument_MustBeDerivedFrom");
      return Compare(x1, y1);
    }
  }
}
