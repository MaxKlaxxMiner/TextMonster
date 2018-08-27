// ReSharper disable MemberCanBeInternal
// ReSharper disable InconsistentNaming

namespace ngMax.Xml
{
  /// <summary>
  /// Basis-Klasse für einen Xml-Knoten
  /// </summary>
  public abstract class X_Node : X_Object
  {
    internal X_Node next;

    internal abstract X_Node CloneNode();
  }
}
