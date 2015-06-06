#region # using *.*

using System.Collections.Generic;

#endregion

namespace TextMonsterSystem.Memory
{
  /// <summary>
  /// TextMemory mit minimalster Technik (langsam)
  /// </summary>
  public sealed class TextMemorySimple : ITextMemory
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich die Daten im Arbeitsspeicher
    /// </summary>
    internal List<char> mem;

    /// <summary>
    /// merkt sich die aktuelle Speicher-Revision
    /// </summary>
    internal long memRev;
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

    #region # // --- Properties und Methoden ---

    #region # // --- public long Length... und Size... ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public long Length { get { return (long)mem.Count; } }

    /// <summary>
    /// gibt die Anzahl der gespeicherten Zeichen als veränderbare MemoryPos zurück
    /// </summary>
    public MemoryPos LengthP { get { return new MemoryPos { pos = Length, rev = memRev }; } }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public long LengthLimit { get { return 536870912L; } }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public long ByteUsedRam { get { return (long)mem.Capacity * 2L; } }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public long ByteUsedDrive { get { return 0; } }

    #endregion

    #region # // --- public 
    /// <summary>
    /// aktualisiert eine Speicherposition auf die aktuelle interne Revision (sofern notwendig)
    /// </summary>
    /// <param name="memPos">Speicher-Position, welche aktualisiert werden soll</param>
    public void UpdateMemoryPos(ref MemoryPos memPos)
    {

    }
    #endregion

    #endregion



  }
}
