// ReSharper disable MemberCanBeInternal
// ReSharper disable InconsistentNaming

namespace ngMax.Xml
{
  /// <summary>
  /// Klasse für ein Text-Element
  /// </summary>
  public class X_Text : X_Node
  {
    internal string text;

    internal override X_Node CloneNode()
    {
      return new X_Text(this);
    }

    X_Text(X_Text other)
    {
      text = other.text;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="value">Wert, welcher hinzugefügt werden soll</param>
    public X_Text(string value)
    {
      text = value;
    }
  }
}
