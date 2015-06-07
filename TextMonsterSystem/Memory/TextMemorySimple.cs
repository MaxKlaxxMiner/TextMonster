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
    /// merkt sich die aktuelle Speicher-Revision
    /// </summary>
    long memRev;

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
    public void Dispose()
    {
      mem = null;
      memLog = null;
    }
    #endregion

    #region # // --- Properties und Methoden ---

    #region # // --- public long Length... und Size... ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public long Length { get { return (long)mem.Count; } }

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
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public MemoryPos Insert(long offset, char value)
    {
      return this.Insert(new MemoryPos { pos = offset, rev = memRev }, value);
    }

    /// <summary>
    /// fügt ein Zeichenarray in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichenarray eingefügt werden soll</param>
    /// <param name="values">die Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichenarrays</returns>
    public MemoryPos Insert(long offset, char[] values)
    {
      return this.Insert(new MemoryPos { pos = offset, rev = memRev }, values);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public MemoryPos Insert(long offset, IEnumerable<char> values)
    {
      return this.Insert(new MemoryPos { pos = offset, rev = memRev }, values);
    }

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public MemoryPos Insert(MemoryPos offset, char value)
    {
      Debug.Assert(offset.pos <= Length, "OutOfRange?");
      UpdateMemoryPos(ref offset);

      mem.Insert((int)offset.pos, value);

      memLog.Add(new MemLog { pos = offset.pos, dif = 1 });
      memRev++;

      return new MemoryPos { pos = offset.pos + 1, rev = memRev };
    }

    /// <summary>
    /// fügt ein Zeichenarray in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo das Zeichenarray eingefügt werden soll</param>
    /// <param name="values">die Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichenarrays</returns>
    public MemoryPos Insert(MemoryPos offset, char[] values) 
    {
      return this.Insert(offset, values.AsEnumerable());
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="offset">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public MemoryPos Insert(MemoryPos offset, IEnumerable<char> values)
    {
      foreach (char c in values)
      {
        offset = Insert(offset, c);
      }
      return offset;
    }
    #endregion

    #endregion
  }
}
