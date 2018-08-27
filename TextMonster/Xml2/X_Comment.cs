// ReSharper disable MemberCanBeInternal
// ReSharper disable InconsistentNaming

namespace ngMax.Xml
{
  /// <summary>
  /// Klasse für einen Xml-Kommentar
  /// </summary>
  public sealed class X_Comment : X_Node
  {
    readonly string value;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="value">Kommentar-Inhalt</param>
    public X_Comment(string value)
    {
      this.value = value;
    }

    X_Comment(X_Comment other)
    {
      value = other.value;
    }

    internal override X_Node CloneNode()
    {
      return new X_Comment(this);
    }
  }
}
