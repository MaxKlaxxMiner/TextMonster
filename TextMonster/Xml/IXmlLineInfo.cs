
namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine Schnittstelle bereit, über die eine Klasse Zeilen- und Positionsinformationen zurückgeben kann.
  /// </summary>
  public interface IXmlLineInfo
  {
    /// <summary>
    /// Ruft die aktuelle Zeilennummer ab.
    /// </summary>
    /// 
    /// <returns>
    /// Die aktuelle Zeilennummer oder 0, wenn keine Zeileninformationen vorliegen (z. B. gibt <see cref="M:System.Xml.IXmlLineInfo.HasLineInfo"/>false zurück).
    /// </returns>
    int LineNumber { get; }

    /// <summary>
    /// Ruft die aktuelle Zeilenposition ab.
    /// </summary>
    /// 
    /// <returns>
    /// Die aktuelle Zeilenposition oder 0, wenn keine Zeileninformationen vorliegen (z. B. gibt <see cref="M:System.Xml.IXmlLineInfo.HasLineInfo"/>false zurück).
    /// </returns>
    int LinePosition { get; }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob die Klasse Zeileninformationen zurückgeben kann.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn <see cref="P:System.Xml.IXmlLineInfo.LineNumber"/> und <see cref="P:System.Xml.IXmlLineInfo.LinePosition"/> angegeben werden können, andernfalls false.
    /// </returns>
    bool HasLineInfo();
  }
}
