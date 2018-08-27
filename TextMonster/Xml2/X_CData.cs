// ReSharper disable MemberCanBeInternal
// ReSharper disable InconsistentNaming

namespace ngMax.Xml
{
  /// <summary>
  /// CData-Objekt
  /// </summary>
  public sealed class X_CData : X_Text
  {
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="value">Inhalt im CData-Objekt</param>
    public X_CData(string value)
      : base(value)
    {
    }
  }
}
