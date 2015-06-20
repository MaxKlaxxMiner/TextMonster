#region # using *.*

using System;
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

    /// <summary>
    /// gibt an, ob die Speicherposition gültig ist oder setzt diese (kann nur auf "false" gesetzt werden)
    /// </summary>
    public bool Valid
    {
      get
      {
        return rev >= 0;
      }
      set
      {
        if (value)
        {
          if (!Valid) throw new ArgumentException();
        }
        else
        {
          rev = -1;
        }
      }
    }
  }
}
