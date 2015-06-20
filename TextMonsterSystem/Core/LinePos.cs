#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace TextMonsterSystem.Core
{
  /// <summary>
  /// Struktur zum speicher der Position einer Textzeile (mit Anfang und Ende)
  /// </summary>
  public struct LinePos
  {
    /// <summary>
    /// Anfangposition der Zeile
    /// </summary>
    internal MemoryPos lineStart;
    /// <summary>
    /// Endposition der Zeile
    /// </summary>
    internal MemoryPos lineEnd;

    /// <summary>
    /// gibt an, ob die Speicherposition gültig ist oder setzt diese (kann nur auf "false" gesetzt werden)
    /// </summary>
    public bool Valid
    {
      get
      {
        return lineStart.Valid && lineEnd.Valid;
      }
      set
      {
        lineStart.Valid = lineEnd.Valid = value;
      }
    }
  }
}
