#region # using *.*

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
    List<char> mem;

    /// <summary>
    /// merkt sich die Größen-Änderungen im Speicher
    /// </summary>
    List<MemLog> memLog;

    /// <summary>
    /// Datensatz eines Speicher-Logs
    /// </summary>
    struct MemLog
    {
      /// <summary>
      /// Speicherposition, wo etwas geändert wurde
      /// </summary>
      public long pos;
      /// <summary>
      /// Speicheränderung (positiv = eingefügte Zeichen, negativ = gelöschte Zeichen)
      /// </summary>
      public long dif;
    }
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    public TextMemorySimple()
    {
      mem = new List<char>();
      memLog = new List<MemLog>();
    }

    /// <summary>
    /// gibt alle Ressourcen wieder frei
    /// </summary>
    public override void Dispose()
    {
      mem = null;
      memLog = null;
    }
    #endregion

    #region # // --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public override long Length { get { return (long)mem.Count; } }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public override long LengthLimit { get { return 536870912L; } }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public override long ByteUsedRam { get { return (long)mem.Capacity * 2L; } }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public override long ByteUsedDrive { get { return 0; } }
    #endregion

    #region # // --- Methoden ---
    /// <summary>
    /// aktualisiert eine Speicherposition auf die aktuelle interne Revision (sofern notwendig)
    /// </summary>
    /// <param name="memPos">Speicher-Position, welche aktualisiert werden soll</param>
    public override void UpdateMemoryPos(ref MemoryPos memPos)
    {
      while (memPos.rev < memRev)
      {
        var log = memLog[(int)memPos.rev];
        memPos.rev++;

        if (log.pos >= memPos.pos) continue; // Log-Eintrag hat nur dahinter liegenden Speicher geändert -> überspringen

        memPos.pos += log.dif; // dahinter liegenden Speicher anpassen

        if (log.dif < 0 && memPos.pos < log.pos) memPos.pos = log.pos; // Speicherposition war innerhalb eines gelöschten Bereiches
      }
    }

    /// <summary>
    /// wandelt eine Speicherposition in eine Zeichen-Position um
    /// </summary>
    /// <param name="offset">Speicherposition, welche umgerechnet werden soll</param>
    /// <returns>entsprechende Zeichenposition</returns>
    public override long GetCharPos(MemoryPos offset)
    {
      UpdateMemoryPos(ref offset);
      return offset.pos;
    }

    /// <summary>
    /// wandelt eine Zeichenposition in eine Speicherposition um
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche umgerechnet werden soll</param>
    /// <returns>entsprechende Speicherposition</returns>
    public override MemoryPos GetMemoryPos(long charPos)
    {
      return new MemoryPos { pos = charPos, rev = memRev };
    }

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(MemoryPos offset, char value)
    {
      UpdateMemoryPos(ref offset);
      mem.Insert((int)offset.pos, value);

      memLog.Add(new MemLog { pos = offset.pos, dif = 1 });
      memRev++;

      return new MemoryPos { pos = offset.pos + 1, rev = memRev };
    }

    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="offset">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="end">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public override void Remove(MemoryPos offset, MemoryPos end)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

}
