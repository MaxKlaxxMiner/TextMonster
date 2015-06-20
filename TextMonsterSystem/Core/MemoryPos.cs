#region # using *.*

using System;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterSystem.Core
{
  /// <summary>
  /// merkt sich die Speicherposition in einer TextMemory-Klasse
  /// </summary>
  public class MemoryPos
  {
    /// <summary>
    /// merkt sich die aktuelle Revisions-Nummer, wo die Position zugeordnet war
    /// </summary>
    internal long rev;
    /// <summary>
    /// merkt sich die aktuelle absolute Position im TextSpeicher
    /// </summary>
    internal long pos;
  }
}
