#region # using *.*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace TextMonster.TextMemory
{
  /// <summary>
  /// TextMemory mit minimalster Technik (langsam)
  /// </summary>
  public class TextMemorySimple : ITextMemory
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich die Daten im Arbeitsspeicher
    /// </summary>
    public List<char> mem;
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    public TextMemorySimple()
    {
      mem = new List<char>();
    }

    /// <summary>
    /// gibt alle Ressourcen wieder frei
    /// </summary>
    public void Dispose()
    {
      mem = null;
    }
    #endregion

    #region # // --- Properties ---

    #region # // --- public long Length... und Size... ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public long Length { get { return (long)mem.Count; } }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public long LengthMaximum { get { return 536870912L; } }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public long ByteUsedRam { get { return (long)mem.Capacity * 2L; } }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public long ByteUsedDrive { get { return 0; } }

    #endregion

    #endregion
  }
}
