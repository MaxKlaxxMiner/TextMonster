#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem.Core;
using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterSystem
{
  /// <summary>
  /// Textmonster-Klasse mit Basisfunktionalität
  /// </summary>
  public sealed class TextMonsterSimpleFull : ITextMonster
  {
    #region # // --- Variablen ---
    /// <summary>
    /// Klasse, welche sich den Textspeicher merkt
    /// </summary>
    ITextMemory mem;
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    public TextMonsterSimpleFull()
    {
      mem = new TextMemorySimpleFull();
    }

    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public override void Dispose()
    {
      mem.Dispose();
    }
    #endregion

    #region # // --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public override long Length { get { return mem.Length; } }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public override long LengthLimit { get { return mem.LengthLimit; } }

    /// <summary>
    /// gibt die Anzahl der Zeilen zurück
    /// </summary>
    public override long Lines { get { return 1L; } }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public override long ByteUsedRam { get { return mem.ByteUsedRam; } }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public override long ByteUsedDrive { get { return mem.ByteUsedDrive; } }
    #endregion

    #region # // --- Methoden ---

    #region # // --- GetPos() ---
    /// <summary>
    /// gibt die Speicherposition anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public override MemoryPos GetMemoryPos(long charPos)
    {
      return mem.GetMemoryPos(charPos);
    }

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public override long GetCharPos(MemoryPos memPos)
    {
      return mem.GetCharPos(memPos);
    }
    #endregion

    #region # // --- Insert() ---
    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(MemoryPos memPos, char value)
    {
      return mem.Insert(memPos, value);
    }

    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Zeichenposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(long charPos, char value)
    {
      return mem.Insert(charPos, value);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(MemoryPos memPos, IEnumerable<char> values)
    {
      return mem.Insert(memPos, values);
    }

    /// <summary>
    /// fügt eine Liste von Zeichen in den Speicher ein
    /// </summary>
    /// <param name="charPos">Startposition, wo die Zeichen eingefügt werden sollen</param>
    /// <param name="values">Enumerable der Zeichen, welche eingefügt werden sollen</param>
    /// <returns>neue Speicherposition am Ende der eingefügten Zeichen</returns>
    public override MemoryPos Insert(long charPos, IEnumerable<char> values)
    {
      return mem.Insert(charPos, values);
    }
    #endregion

    #region # // --- Remove() ---
    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="memPosStart">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="memPosEnd">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public override void Remove(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      mem.Remove(memPosStart, memPosEnd);
    }

    /// <summary>
    /// löscht bestimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="charPos">Startposition, wo Zeichen im Speicher gelöscht werden sollen</param>
    /// <param name="length">Anzahl der Zeichen, welche gelöscht werden sollen</param>
    public override void Remove(long charPos, long length)
    {
      mem.Remove(charPos, length);
    }
    #endregion

    #region # // --- GetChars() ---
    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="memPosStart">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <param name="memPosEnd">Endposition, der Zeichen im Speicher (exklusive)</param>
    /// <returns>Enumerable mit den entsprechenden Zeichen</returns>
    public override IEnumerable<char> GetChars(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      return mem.GetChars(memPosStart, memPosEnd);
    }
    #endregion

    #region # // --- GetLine() ---
    /// <summary>
    /// gibt den Anfang einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf den Anfang der Zeile</returns>
    public override MemoryPos GetLineStart(MemoryPos memPos)
    {
      return mem.GetLineStart(memPos);
    }

    /// <summary>
    /// gibt das Ende einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf das Ende der Zeile (hinter dem letzten Zeichen)</returns>
    public override MemoryPos GetLineEnd(MemoryPos memPos)
    {
      return mem.GetLineEnd(memPos);
    }

    /// <summary>
    /// gibt Position einer gesamten Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Position der gesamten Zeile (Anfang und Ende der Zeile)</returns>
    public override LinePos GetLine(MemoryPos memPos)
    {
      return new LinePos { lineStart = mem.GetLineStart(memPos), lineEnd = mem.GetLineEnd(memPos) };
    }

    /// <summary>
    /// gibt die Position der nachfolgenden Zeile zurück
    /// </summary>
    /// <param name="linePos">vorherige Zeilenposition</param>
    /// <returns>nachfolgende Zeilenposition</returns>
    public override LinePos GetNextLine(LinePos linePos)
    {
      if (!linePos.Valid) return LinePos.InvalidPos;

      long p = mem.GetCharPos(linePos.lineEnd) + 1;
      if (p > mem.Length) return LinePos.InvalidPos;
      if (p < mem.Length && mem.GetChars(p, 1).First() == '\r') p++;

      MemoryPos nextLineStart = mem.GetMemoryPos(p);
      return new LinePos { lineStart = nextLineStart, lineEnd = mem.GetLineEnd(nextLineStart) };
    }

    /// <summary>
    /// gibt die Position der vorhergehenden Zeile zurück
    /// </summary>
    /// <param name="linePos">bisherige Zeilenposition</param>
    /// <returns>vorhergehende Zeilenposition</returns>
    public override LinePos GetPrevLine(LinePos linePos)
    {
      if (!linePos.Valid) return LinePos.InvalidPos;

      long p = mem.GetCharPos(linePos.lineStart) - 1;
      if (p < 0) return LinePos.InvalidPos;
      if (p > 0 && mem.GetChars(p - 1, 1).First() == '\r') p--;

      MemoryPos prevLineEnd = mem.GetMemoryPos(p);
      return new LinePos { lineStart = mem.GetLineStart(prevLineEnd), lineEnd = prevLineEnd };
    }

    /// <summary>
    /// gibt die Position einer bestimmten Zeile zurück
    /// </summary>
    /// <param name="lineNumber">Zeilennummer, welche zurück gegeben werden soll (beginnend bei 0)</param>
    /// <returns>entsprechende Zeilenposition</returns>
    public override LinePos GetLine(long lineNumber)
    {
      if (lineNumber < 0) return LinePos.InvalidPos;

      long p = 0;
      long memLength = mem.Length;
      while (lineNumber > 0)
      {
        p = mem.GetCharPos(mem.GetLineEnd(mem.GetMemoryPos(p))) + 1;
        if (p > memLength) return LinePos.InvalidPos;
        if (p < memLength && mem.GetChars(p, 1).First() == '\r') p++;
        lineNumber--;
      }

      MemoryPos lineStart = mem.GetMemoryPos(p);
      return new LinePos { lineStart = lineStart, lineEnd = mem.GetLineEnd(lineStart) };
    }
    #endregion

    #endregion

  }
}
