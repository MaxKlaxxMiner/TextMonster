using System;

namespace TextMonster.Xml
{
  /// <summary>
  /// Gibt Serialisierungsoptionen an.
  /// </summary>
  [Flags]
  public enum SaveOptions
  {
    None = 0,
    DisableFormatting = 1,
    OmitDuplicateNamespaces = 2
  }
}
