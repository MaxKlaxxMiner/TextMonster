using System;

namespace TextMonster.Xml
{
  /// <summary>
  /// Gibt Ladeoptionen beim Analysieren von XML an.
  /// </summary>
  [Flags]
  public enum LoadOptions
  {
    None = 0,
    PreserveWhitespace = 1,
    SetBaseUri = 2,
    SetLineInfo = 4,
  }
}
