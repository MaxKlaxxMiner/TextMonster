using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Knoten oder ein Attribut in einer XML-Struktur dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public abstract class X_Object
  {
    internal X_Container parent;

    /// <summary>
    /// Ruft das <see cref="T:System.Xml.Linq.XDocument"/> für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.Xml.Linq.XDocument"/> für dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public X_Document Document
    {
      get
      {
        var xobject = this;
        while (xobject.parent != null)
          xobject = xobject.parent;
        return xobject as X_Document;
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public abstract XmlNodeType NodeType { get; }

    /// <summary>
    /// Ruft das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public X_Element Parent
    {
      get
      {
        return parent as X_Element;
      }
    }
  }
}
