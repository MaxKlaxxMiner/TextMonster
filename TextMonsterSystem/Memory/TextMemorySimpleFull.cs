#region # using *.*

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using TextMonsterSystem.Core;

#endregion

namespace TextMonsterSystem.Memory
{
  /// <summary>
  /// TextMemory mit minimalster Technik (langsam)
  /// </summary>
  public sealed class TextMemorySimpleFull : ITextMemory
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich die Daten im Arbeitsspeicher
    /// </summary>
    internal List<char> mem;

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
      /// <summary>
      /// gibt den Inhalt als lesbare Zeichenkette aus
      /// </summary>
      /// <returns>lesbare Zeichenkette</returns>
      public override string ToString()
      {
        return (new { pos, dif }).ToString();
      }
    }
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    public TextMemorySimpleFull()
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
    public override void UpdateMemoryPos(MemoryPos memPos)
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
    /// <param name="memPos">Speicherposition, welche umgerechnet werden soll</param>
    /// <returns>entsprechende Zeichenposition</returns>
    public override long GetCharPos(MemoryPos memPos)
    {
      UpdateMemoryPos(memPos);
      return memPos.pos;
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
    /// <param name="memPos">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(MemoryPos memPos, char value)
    {
      UpdateMemoryPos(memPos);
      mem.Insert((int)memPos.pos, value);

      memLog.Add(new MemLog { pos = memPos.pos, dif = 1 });
      memRev++;

      return new MemoryPos { pos = memPos.pos + 1, rev = memRev };
    }

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Zeichenposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(long charPos, char value)
    {
      mem.Insert((int)charPos, value);

      memLog.Add(new MemLog { pos = charPos, dif = 1 });
      memRev++;

      return new MemoryPos { pos = charPos + 1, rev = memRev };
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(MemoryPos memPos, IEnumerable<char> values)
    {
      UpdateMemoryPos(memPos);

      int oldCount = mem.Count;
      mem.InsertRange((int)memPos.pos, values);

      long length = mem.Count - oldCount;

      memLog.Add(new MemLog { pos = memPos.pos, dif = length });
      memRev++;

      return new MemoryPos { pos = memPos.pos + length, rev = memRev };
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(long charPos, IEnumerable<char> values)
    {
      int oldCount = mem.Count;
      mem.InsertRange((int)charPos, values);

      long length = mem.Count - oldCount;

      memLog.Add(new MemLog { pos = charPos, dif = length });
      memRev++;

      return new MemoryPos { pos = charPos + length, rev = memRev };
    }

    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="memPosStart">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="memPosEnd">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public override void Remove(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      UpdateMemoryPos(memPosStart);
      UpdateMemoryPos(memPosEnd);
      long length = memPosEnd.pos - memPosStart.pos;
      mem.RemoveRange((int)memPosStart.pos, (int)length);

      memLog.Add(new MemLog { pos = memPosStart.pos, dif = -length });
      memRev++;
    }

    /// <summary>
    /// löscht bestimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="charPos">Startposition, wo Zeichen im Speicher gelöscht werden sollen</param>
    /// <param name="length">Anzahl der Zeichen, welche gelöscht werden sollen</param>
    public override void Remove(long charPos, long length)
    {
      mem.RemoveRange((int)charPos, (int)length);

      memLog.Add(new MemLog { pos = charPos, dif = -length });
      memRev++;
    }

    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <returns>Enumerable der entsprechenden Zeichen</returns>
    public override IEnumerable<char> GetChars(MemoryPos memPos)
    {
      UpdateMemoryPos(memPos);
      for (int p = (int)memPos.pos; p < mem.Count; p++)
      {
        yield return mem[p];
      }
    }

    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override char[] GetChars(long charPos, long length)
    {
      char[] result = new char[length];

      for (int i = 0; i < result.Length; i++)
      {
        result[i] = mem[(int)charPos++];
      }

      return result;
    }

    /// <summary>
    /// gibt den Anfang einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf den Anfang der Zeile</returns>
    public override MemoryPos GetLineStart(MemoryPos memPos)
    {
      UpdateMemoryPos(memPos);
      long pos = memPos.pos;

      while (pos > 0 && mem[(int)pos - 1] != '\n') pos--;

      return new MemoryPos { pos = pos, rev = memRev };
    }

    /// <summary>
    /// gibt das Ende einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf das Ende der Zeile (hinter dem letzten Zeichen)</returns>
    public override MemoryPos GetLineEnd(MemoryPos memPos)
    {
      UpdateMemoryPos(memPos);
      long pos = memPos.pos;

      while (pos < mem.Count && mem[(int)pos] != '\n') pos++;
      if (pos > 0 && mem[(int)pos - 1] == '\r') pos--;

      return new MemoryPos { pos = pos, rev = memRev };
    }

    /// <summary>
    /// löscht alle Zeichen aus dem Speicher
    /// </summary>
    public override void Clear()
    {
      memLog.Add(new MemLog { pos = 0, dif = -mem.Count });
      memRev++;

      mem.Clear();
    }

    #endregion

  }

}
