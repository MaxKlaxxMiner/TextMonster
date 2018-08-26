﻿using System;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt Daten für das <see cref="E:System.Xml.Linq.XObject.Changing"/>-Ereignis und das <see cref="E:System.Xml.Linq.XObject.Changed"/>-Ereignis bereit.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public class XObjectChangeEventArgs : EventArgs
  {
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Add"/>-Änderungsereignis.
    /// </summary>
    public static readonly XObjectChangeEventArgs Add = new XObjectChangeEventArgs(XObjectChange.Add);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Remove"/>-Änderungsereignis.
    /// </summary>
    public static readonly XObjectChangeEventArgs Remove = new XObjectChangeEventArgs(XObjectChange.Remove);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Name"/>-Änderungsereignis.
    /// </summary>
    public static readonly XObjectChangeEventArgs Name = new XObjectChangeEventArgs(XObjectChange.Name);
    /// <summary>
    /// Ereignisargument für ein <see cref="F:System.Xml.Linq.XObjectChange.Value"/>-Änderungsereignis.
    /// </summary>
    public static readonly XObjectChangeEventArgs Value = new XObjectChangeEventArgs(XObjectChange.Value);
    private XObjectChange objectChange;

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
        return this.objectChange;
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="T:System.Xml.Linq.XObjectChangeEventArgs"/>-Klasse.
    /// </summary>
    /// <param name="objectChange">Ein <see cref="T:System.Xml.Linq.XObjectChange"/>, das die Ereignisargumente für LINQ to XML-Ereignisse enthält.</param>
    public XObjectChangeEventArgs(XObjectChange objectChange)
    {
      this.objectChange = objectChange;
    }
  }
}
