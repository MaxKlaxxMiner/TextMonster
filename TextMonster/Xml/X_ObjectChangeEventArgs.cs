using System;
using System.Xml.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt Daten für das <see cref="E:System.Xml.Linq.XObject.Changing"/>-Ereignis und das <see cref="E:System.Xml.Linq.XObject.Changed"/>-Ereignis bereit.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public class X_ObjectChangeEventArgs : EventArgs
  {
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Add"/>-Änderungsereignis.
    /// </summary>
    public static readonly X_ObjectChangeEventArgs Add = new X_ObjectChangeEventArgs(XObjectChange.Add);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Remove"/>-Änderungsereignis.
    /// </summary>
    public static readonly X_ObjectChangeEventArgs Remove = new X_ObjectChangeEventArgs(XObjectChange.Remove);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Name"/>-Änderungsereignis.
    /// </summary>
    public static readonly X_ObjectChangeEventArgs Name = new X_ObjectChangeEventArgs(XObjectChange.Name);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Value"/>-Änderungsereignis.
    /// </summary>
    public static readonly X_ObjectChangeEventArgs Value = new X_ObjectChangeEventArgs(XObjectChange.Value);
    readonly XObjectChange objectChange;

    /// <summary>
    /// Ruft den Typ der Änderung ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Linq.XObjectChange"/>, das den Typ der Änderung enthält.
    /// </returns>
    public XObjectChange ObjectChange
    {
      get
      {
        return objectChange;
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XObjectChangeEventArgs"/>-Klasse.
    /// </summary>
    /// <param name="objectChange">Ein <see cref="T:System.Xml.Linq.XObjectChange"/>, das die Ereignisargumente für LINQ to XML-Ereignisse enthält.</param>
    public X_ObjectChangeEventArgs(XObjectChange objectChange)
    {
      this.objectChange = objectChange;
    }
  }
}
