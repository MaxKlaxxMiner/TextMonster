﻿#region # using *.*

using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterSystem.Core
{
  /// <summary>
  /// merkt sich die Speicherposition in einer TextMemory-Klasse
  /// </summary>
  public struct MemoryPos
  {
    /// <summary>
    /// merkt sich die aktuelle Revisions-Nummer, wo die Position zugeordnet war
    /// </summary>
    internal long rev;
    /// <summary>
    /// merkt sich die aktuelle absolute Position im TextSpeicher
    /// </summary>
    internal long pos;

    /// <summary>
    /// gibt die eigene absolute Speicherposition zurück
    /// </summary>
    /// <param name="mem">aktuelle TextMemory-Klasse, welche für die Speicherverwaltung zuständig ist</param>
    /// <returns>aktuelle reale Speicherposition TextMemory</returns>
    internal long GetPos(ITextMemory mem)
    {
      mem.UpdateMemoryPos(ref this);
      return pos;
    }
  }
}