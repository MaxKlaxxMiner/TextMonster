// ReSharper disable InconsistentNaming

// ReSharper disable MemberCanBeInternal
namespace ngMax.Xml
{
  /// <summary>
  /// Xml-Attribut
  /// </summary>
  public sealed class X_Attribute : X_Object
  {
    internal X_Attribute next;
    internal readonly string name;
    internal readonly string value;

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="other">anderes Attribut, welches kopiert werden soll</param>
    public X_Attribute(X_Attribute other)
    {
      name = other.name;
      value = other.value;
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="name">Name des Attributes</param>
    /// <param name="value">Inhalt des Attributes</param>
    public X_Attribute(string name, object value)
    {
      string stringValue = X_Container.GetStringValue(value);
      this.name = name;
      this.value = stringValue;
    }
  }
}
